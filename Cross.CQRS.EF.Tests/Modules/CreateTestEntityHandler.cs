using Cross.CQRS.EF.Tests.Modules.Events;

namespace Cross.CQRS.EF.Tests.Modules;

public class CreateTestEntityHandler : CommandHandler<CreateTestEntityCommand>
{
    private readonly TestDbContext _dbContext;

    public CreateTestEntityHandler(ICommandEventQueueWriter commandEvents, ILogger<CreateTestEntityHandler> logger, TestDbContext dbContext)
        : base(commandEvents, logger)
    {
        _dbContext = dbContext;
    }

    protected override async Task HandleAsync(CreateTestEntityCommand command, CancellationToken cancellationToken)
    {
        var entity = new TestEntity
        {
            Name = command.Name,
            CreatedOn = DateTime.UtcNow
        };

        CommandEvents.Write(new TestEvent { CommandId = command.CommandId });

        await _dbContext.TestEntities.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
