namespace Cross.CQRS.EF.Behaviors;

internal sealed class TransactionalBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
{
    private readonly IDbContextProvider _dbContextProvider;
    private readonly IHandlerLocator _handlerLocator;

    public TransactionalBehavior(IHandlerLocator handlerLocator, IDbContextProvider dbContextProvider)
    {
        _handlerLocator = handlerLocator;
        _dbContextProvider = dbContextProvider;
    }

    /// <inheritdoc />
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is not ICommand<TResponse>)
        {
            return await next();
        }

        var handler = _handlerLocator.FindHandlerTypeByRequest(typeof(TRequest));
        if (handler != null)
        {
            var isExplicitTransactionSet = handler
                .GetCustomAttributes(typeof(ExplicitTransactionAttribute), inherit: false)
                .Any();

            if (isExplicitTransactionSet)
            {
                // Skip behavior if requested explicit transaction management.
                return await next();
            }
        }

        var dbContext = _dbContextProvider.Get();
        var executionStrategy = dbContext.Database.CreateExecutionStrategy();

        TResponse response = default;

        await executionStrategy.ExecuteAsync(async () =>
        {
            using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            response = await next();
            transaction.Commit();
        });

        // Clean-up tracked entries
        var trackedEntries = dbContext.ChangeTracker.Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToArray();

        foreach (var entry in trackedEntries)
        {
            entry.State = EntityState.Detached;
        }

        return response;
    }
}
