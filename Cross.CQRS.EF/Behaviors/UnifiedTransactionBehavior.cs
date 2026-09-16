namespace Cross.CQRS.EF.Behaviors;

internal sealed class UnifiedTransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
{
    private readonly IDbContextProvider _dbContextProvider;
    private readonly IHandlerLocator _handlerLocator;
    private readonly IOptions<TransactionBehaviorOptions> _options;

    public UnifiedTransactionBehavior(IHandlerLocator handlerLocator, IDbContextProvider dbContextProvider, IOptions<TransactionBehaviorOptions> options)
    {
        _handlerLocator = handlerLocator;
        _dbContextProvider = dbContextProvider;
        _options = options;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is not ICommand<TResponse>)
        {
            return await next().ConfigureAwait(false);
        }

        var behavior = _options.Value.Behavior;
        var isolationLevel = _options.Value.IsolationLevel;

        var handler = _handlerLocator.FindHandlerTypeByRequest(typeof(TRequest));
        if (handler != null)
        {
            var exactTransactionAttribute = handler
                .GetCustomAttributes(typeof(ExactTransactionAttribute), inherit: false)
                .FirstOrDefault() as ExactTransactionAttribute;

            if (exactTransactionAttribute != null)
            {
                behavior = exactTransactionAttribute.TransactionBehavior;
                isolationLevel = exactTransactionAttribute.IsolationLevel;
            }
        }

        var dbContext = _dbContextProvider.Get();

        try
        {
            return behavior switch
            {
                TransactionBehaviorEnum.TransactionalBehavior => await HandleTransactionalBehaviorAsync(next, isolationLevel, dbContext, cancellationToken).ConfigureAwait(false),
                TransactionBehaviorEnum.ScopeBehavior => await HandleScopeBehaviorAsync(next, isolationLevel).ConfigureAwait(false),
                TransactionBehaviorEnum.TransactionalScopeBehavior => await HandleTransactionalScopeBehaviorAsync(next, isolationLevel, dbContext, cancellationToken).ConfigureAwait(false),
                TransactionBehaviorEnum.NoBehavior =>
                    // Skip behavior if not correspond the TransactionBehaviorEnum or not set.
                    await next().ConfigureAwait(false),
                _ => default
            };
        }
        finally
        {
            // Detach pending entries even when next/commit throws, so the scoped DbContext does not keep a failed command graph.
            var trackedEntries = dbContext.ChangeTracker.Entries()
                .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
                .ToArray();
            foreach (var entry in trackedEntries)
            {
                entry.State = EntityState.Detached;
            }
        }
    }

    private async Task<TResponse> HandleTransactionalBehaviorAsync(RequestHandlerDelegate<TResponse> next, IsolationLevel isolationLevel, DbContext dbContext, CancellationToken cancellationToken)
    {
        TResponse response = default;

        var executionStrategy = dbContext.Database.CreateExecutionStrategy();

        await executionStrategy.ExecuteAsync(async () =>
        {
            var transaction = await dbContext.Database
                .BeginTransactionAsync(isolationLevel.ToDataIsolation(), cancellationToken)
                .ConfigureAwait(false);
            await using (transaction.ConfigureAwait(false))
            {
                response = await next().ConfigureAwait(false);
                await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
            }
        }).ConfigureAwait(false);

        return response;
    }

    private async Task<TResponse> HandleScopeBehaviorAsync(RequestHandlerDelegate<TResponse> next, IsolationLevel isolationLevel)
    {
        TResponse response = default;

        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = isolationLevel,
            Timeout = TimeSpan.FromSeconds(60)
        };

        using var scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions, TransactionScopeAsyncFlowOption.Enabled);
        response = await next().ConfigureAwait(false);
        scope.Complete();

        return response;
    }

    private async Task<TResponse> HandleTransactionalScopeBehaviorAsync(RequestHandlerDelegate<TResponse> next, IsolationLevel isolationLevel, DbContext dbContext, CancellationToken cancellationToken)
    {
        TResponse response = default;

        var executionStrategy = dbContext.Database.CreateExecutionStrategy();

        await executionStrategy.ExecuteAsync(async () =>
        {
            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = isolationLevel,
                Timeout = TimeSpan.FromSeconds(60)
            };

            using var transactionScope = new TransactionScope(TransactionScopeOption.Required, transactionOptions, TransactionScopeAsyncFlowOption.Enabled);
            response = await next().ConfigureAwait(false);
            transactionScope.Complete();
        }).ConfigureAwait(false);

        return response;
    }
}
