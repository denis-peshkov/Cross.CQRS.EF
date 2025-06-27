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
            return await next();
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
                behavior = exactTransactionAttribute.Value;
            }
        }

        var dbContext = _dbContextProvider.Get();

        var response = behavior switch
        {
            TransactionBehaviorEnum.TransactionalBehavior => await HandleTransactionalBehaviorAsync(next, isolationLevel, dbContext, cancellationToken),
            TransactionBehaviorEnum.ScopeBehavior => await HandleScopeBehaviorAsync(next, isolationLevel),
            TransactionBehaviorEnum.TransactionalScopeBehavior => await HandleTransactionalScopeBehaviorAsync(next, isolationLevel, dbContext, cancellationToken),
            TransactionBehaviorEnum.NoBehavior =>
                // Skip behavior if not correspond the TransactionBehaviorEnum or not set.
                await next(),
            _ => default
        };

        // Cleanup tracked entries
        var trackedEntries = dbContext.ChangeTracker.Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToArray();

        foreach (var entry in trackedEntries)
        {
            entry.State = EntityState.Detached;
        }

        return response;
    }

    private async Task<TResponse> HandleScopeBehaviorAsync(RequestHandlerDelegate<TResponse> next, IsolationLevel isolationLevel)
    {
        TResponse response = default;

        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = (System.Transactions.IsolationLevel)isolationLevel,
            Timeout = TimeSpan.FromSeconds(60)
        };

        using var scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions, TransactionScopeAsyncFlowOption.Enabled);
        response = await next();
        scope.Complete();

        return response;
    }

    private async Task<TResponse> HandleTransactionalBehaviorAsync(RequestHandlerDelegate<TResponse> next, IsolationLevel isolationLevel, DbContext dbContext, CancellationToken cancellationToken)
    {
        TResponse response = default;

        var executionStrategy = dbContext.Database.CreateExecutionStrategy();

        await executionStrategy.ExecuteAsync(async () =>
        {
            await dbContext.Database.OpenConnectionAsync(cancellationToken);
            await using var transaction = await dbContext.Database
                .GetDbConnection()
                .BeginTransactionAsync(isolationLevel, cancellationToken);
            await dbContext.Database.UseTransactionAsync(transaction, cancellationToken);
            response = await next();
            await transaction.CommitAsync(cancellationToken);
        });

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
                IsolationLevel = (System.Transactions.IsolationLevel)isolationLevel,
                Timeout = TimeSpan.FromSeconds(60)
            };

            using var transactionScope = new TransactionScope(TransactionScopeOption.Required, transactionOptions, TransactionScopeAsyncFlowOption.Enabled);
            response = await next();
            transactionScope.Complete();
        });

        return response;
    }
}
