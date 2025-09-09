namespace Cross.CQRS.EF.Tests.Modules;

public class UpdateTestEntityHandler : CommandHandler<UpdateTestEntityCommand>
{

    private readonly TestDbContext _dbContext;

    public UpdateTestEntityHandler(
        ICommandEventQueueWriter commandEvents,
        ILogger<UpdateTestEntityHandler> logger,
        TestDbContext dbContext)
        : base(commandEvents, logger)
    {
        _dbContext = dbContext;
    }

    protected override async Task HandleAsync(UpdateTestEntityCommand command, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.TestEntities.FindAsync(new object[] { command.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException($"TestEntity with id {command.Id} not found");
        }

        entity.Name = command.Name;
        await _dbContext.SaveChangesAsync(cancellationToken);

        CommandEvents.Write(new TestEntityUpdatedEvent
        {
            CommandId = command.CommandId,
            Id = entity.Id,
            Name = entity.Name
        });
    }


}
