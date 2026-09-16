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

        if (behavior == TransactionBehaviorEnum.NoBehavior)
        {
            return await next().ConfigureAwait(false);
        }

        var dbContext = _dbContextProvider.Get();
        var trackedBefore = SnapshotTrackedEntities(dbContext);

        try
        {
            return behavior switch
            {
                TransactionBehaviorEnum.TransactionalBehavior => await HandleTransactionalBehaviorAsync(next, isolationLevel, dbContext, cancellationToken).ConfigureAwait(false),
                TransactionBehaviorEnum.ScopeBehavior => await HandleScopeBehaviorAsync(next, isolationLevel).ConfigureAwait(false),
                TransactionBehaviorEnum.TransactionalScopeBehavior => await HandleTransactionalScopeBehaviorAsync(next, isolationLevel, dbContext, cancellationToken).ConfigureAwait(false),
                _ => throw new ArgumentOutOfRangeException(nameof(behavior), behavior, null)
            };
        }
        catch
        {
            // Drop this command's graph only; keep Unchanged/pending entries from earlier in the same request scope.
            RestoreTrackedEntities(dbContext, trackedBefore);
            throw;
        }
    }

    private static IReadOnlyDictionary<object, EntityState> SnapshotTrackedEntities(DbContext dbContext)
    {
        return dbContext.ChangeTracker.Entries()
            .ToDictionary(entry => entry.Entity, entry => entry.State, ReferenceEqualityComparer.Instance);
    }

    private static void RestoreTrackedEntities(DbContext dbContext, IReadOnlyDictionary<object, EntityState> trackedBefore)
    {
        foreach (var pair in trackedBefore)
        {
            var entry = dbContext.Entry(pair.Key);
            if (entry.State == pair.Value)
            {
                continue;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.CurrentValues.SetValues(entry.OriginalValues);
            }

            entry.State = pair.Value;
        }

        foreach (var entry in dbContext.ChangeTracker.Entries().ToList())
        {
            if (!trackedBefore.ContainsKey(entry.Entity))
            {
                entry.State = EntityState.Detached;
            }
        }
    }

    private async Task<TResponse> HandleTransactionalBehaviorAsync(RequestHandlerDelegate<TResponse> next, IsolationLevel isolationLevel, DbContext dbContext, CancellationToken cancellationToken)
    {
        TResponse response = default;

        var executionStrategy = dbContext.Database.CreateExecutionStrategy();

        await executionStrategy.ExecuteAsync(async ct =>
            {
                var transaction = await dbContext.Database
                    .BeginTransactionAsync(isolationLevel.ToDataIsolation(), ct)
                    .ConfigureAwait(false);
                await using (transaction.ConfigureAwait(false))
                {
                    response = await next().ConfigureAwait(false);
                    await transaction.CommitAsync(ct).ConfigureAwait(false);
                }
            }, cancellationToken).ConfigureAwait(false);

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

        await executionStrategy.ExecuteAsync(async ct =>
            {
                ct.ThrowIfCancellationRequested();

                var transactionOptions = new TransactionOptions
                {
                    IsolationLevel = isolationLevel,
                    Timeout = TimeSpan.FromSeconds(60)
                };

                using var transactionScope = new TransactionScope(TransactionScopeOption.Required, transactionOptions, TransactionScopeAsyncFlowOption.Enabled);
                response = await next().ConfigureAwait(false);
                transactionScope.Complete();
            }, cancellationToken).ConfigureAwait(false);

        return response;
    }
}
