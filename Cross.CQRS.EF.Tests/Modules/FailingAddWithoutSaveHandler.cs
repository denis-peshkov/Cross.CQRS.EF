namespace Cross.CQRS.EF.Tests.Modules;

public class FailingAddWithoutSaveHandler : CommandHandler<FailingAddWithoutSaveCommand>
{
    private readonly TestDbContext _dbContext;

    public FailingAddWithoutSaveHandler(ICommandEventQueueWriter commandEvents, ILogger<FailingAddWithoutSaveHandler> logger, TestDbContext dbContext)
        : base(commandEvents, logger)
    {
        _dbContext = dbContext;
    }

    protected override async Task HandleAsync(FailingAddWithoutSaveCommand command, CancellationToken cancellationToken)
    {
        var entity = new TestEntity
        {
            Name = command.Name,
            CreatedOn = DateTime.UtcNow
        };

        await _dbContext.TestEntities.AddAsync(entity, cancellationToken);

        throw new InvalidOperationException("Simulated handler failure after Add, before SaveChanges");
    }
}
