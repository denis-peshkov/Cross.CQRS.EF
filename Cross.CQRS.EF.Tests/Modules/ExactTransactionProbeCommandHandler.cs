namespace Cross.CQRS.EF.Tests.Modules;

[ExactTransaction(TransactionBehaviorEnum.TransactionalBehavior, IsolationLevel.ReadCommitted)]
public class ExactTransactionProbeCommandHandler : CommandHandler<ExactTransactionProbeCommand, TransactionSnapshot>
{
    private readonly TestDbContext _dbContext;

    public ExactTransactionProbeCommandHandler(
        ICommandEventQueueWriter commandEvents,
        ILogger<ExactTransactionProbeCommandHandler> logger,
        TestDbContext dbContext)
        : base(commandEvents, logger)
    {
        _dbContext = dbContext;
    }

    protected override Task<TransactionSnapshot> HandleAsync(ExactTransactionProbeCommand command, CancellationToken cancellationToken)
        => Task.FromResult(TransactionSnapshotFactory.Capture(_dbContext));
}
