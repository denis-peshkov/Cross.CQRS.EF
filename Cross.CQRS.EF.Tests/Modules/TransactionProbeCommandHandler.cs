namespace Cross.CQRS.EF.Tests.Modules;

public class TransactionProbeCommandHandler : CommandHandler<TransactionProbeCommand, TransactionSnapshot>
{
    private readonly TestDbContext _dbContext;

    public TransactionProbeCommandHandler(
        ICommandEventQueueWriter commandEvents,
        ILogger<TransactionProbeCommandHandler> logger,
        TestDbContext dbContext)
        : base(commandEvents, logger)
    {
        _dbContext = dbContext;
    }

    protected override Task<TransactionSnapshot> HandleAsync(TransactionProbeCommand command, CancellationToken cancellationToken)
        => Task.FromResult(TransactionSnapshotFactory.Capture(_dbContext));
}
