namespace Cross.CQRS.EF.Tests.Modules;

public class FailingCreateTestEntityHandler : CommandHandler<CreateTestEntityCommand>
{
    private readonly TestDbContext _dbContext;

    public FailingCreateTestEntityHandler(ICommandEventQueueWriter commandEvents, ILogger<FailingCreateTestEntityHandler> logger, TestDbContext dbContext)
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

        await _dbContext.TestEntities.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        throw new InvalidOperationException("Симулируем ошибку для проверки отката транзакции");
    }
}
