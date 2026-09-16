namespace Cross.CQRS.EF.Tests.Modules;

public class TransactionProbeQueryHandler : QueryHandler<TransactionProbeQuery, TransactionSnapshot>
{
    private readonly TestDbContext _dbContext;

    public TransactionProbeQueryHandler(
        ILogger<TransactionProbeQueryHandler> logger,
        TestDbContext dbContext)
        : base(logger)
    {
        _dbContext = dbContext;
    }

    protected override Task<TransactionSnapshot> HandleAsync(TransactionProbeQuery query, CancellationToken cancellationToken)
        => Task.FromResult(TransactionSnapshotFactory.Capture(_dbContext));
}
