namespace Cross.CQRS.EF.Tests.Modules;

public class FailingMutateTrackedEntitiesHandler : CommandHandler<FailingMutateTrackedEntitiesCommand>
{
    private readonly TestDbContext _dbContext;

    public FailingMutateTrackedEntitiesHandler(
        ICommandEventQueueWriter commandEvents,
        ILogger<FailingMutateTrackedEntitiesHandler> logger,
        TestDbContext dbContext)
        : base(commandEvents, logger)
    {
        _dbContext = dbContext;
    }

    protected override Task HandleAsync(FailingMutateTrackedEntitiesCommand command, CancellationToken cancellationToken)
    {
        foreach (var entity in _dbContext.ChangeTracker.Entries<TestEntity>().Select(entry => entry.Entity).ToList())
        {
            entity.Name = "failed-" + entity.Name;
            entity.BalanceAmount = 999m;
        }

        throw new InvalidOperationException("Simulated handler failure after mutating tracked entities");
    }
}
