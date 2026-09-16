namespace Cross.CQRS.EF.Tests.Modules;

public class AddTestEntityWithoutSaveHandler : CommandHandler<AddTestEntityWithoutSaveCommand>
{
    private readonly TestDbContext _dbContext;

    public AddTestEntityWithoutSaveHandler(
        ICommandEventQueueWriter commandEvents,
        ILogger<AddTestEntityWithoutSaveHandler> logger,
        TestDbContext dbContext)
        : base(commandEvents, logger)
    {
        _dbContext = dbContext;
    }

    protected override async Task HandleAsync(AddTestEntityWithoutSaveCommand command, CancellationToken cancellationToken)
    {
        var entity = new TestEntity
        {
            Name = command.Name,
            CreatedOn = DateTime.UtcNow
        };

        await _dbContext.TestEntities.AddAsync(entity, cancellationToken);
    }
}
