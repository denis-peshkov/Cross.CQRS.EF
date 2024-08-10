namespace Cross.CQRS.EF.Behaviors;

internal sealed class ScopeBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
{
    private readonly IDbContextProvider _dbContextProvider;
    private readonly IHandlerLocator _handlerLocator;

    public ScopeBehavior(IHandlerLocator handlerLocator, IDbContextProvider dbContextProvider)
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

        TResponse response = default;
        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            response = await next();
            scope.Complete();
        }

        var dbContext = _dbContextProvider.Get();

        // Clean-up tracked entries
        var trackedEntries = dbContext.ChangeTracker.Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToList();

        foreach (var entry in trackedEntries)
        {
            entry.State = EntityState.Detached;
        }

        return response;
    }
}
