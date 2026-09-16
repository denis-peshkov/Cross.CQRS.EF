namespace SampleWebApp.Modules.Some.Handlers;

// Override host ScopeBehavior so this SaveChanges command uses EF BeginTransaction.
[ExactTransaction(TransactionBehaviorEnum.TransactionalBehavior)]
public class SomeScopeExternalCommandHandler : CommandHandler<SomeScopeExternalCommand>
{
    private readonly Context _dbContext;

    public SomeScopeExternalCommandHandler(
        ICommandEventQueueWriter commandEvents,
        ILogger<SomeScopeExternalCommandHandler> logger,
        Context dbContext)
        : base(commandEvents, logger)
    {
        _dbContext = dbContext;
    }

    protected override async Task HandleAsync(SomeScopeExternalCommand command, CancellationToken cancellationToken)
    {
        CommandEvents.Write(new ExternalEvent(command.CommandId, $"hello from {nameof(SomeScopeExternalCommandHandler)}"));

        Console.WriteLine($"{nameof(SomeScopeExternalCommandHandler)} do something");

        _dbContext.SampleEntities.Add(new SampleEntity { Name = command.Name });
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
