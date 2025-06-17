namespace SampleWebApp.Modules.Some.Handlers;

// [ExactTransaction(TransactionBehaviorEnum.ScopeBehavior)]
public class SomeScopeExternalCommandHandler : CommandHandler<SomeScopeExternalCommand>
{

    public SomeScopeExternalCommandHandler(IEventQueueWriter eventQueueWriter)
        : base(eventQueueWriter)
    {
    }

    protected override Task HandleAsync(SomeScopeExternalCommand command, CancellationToken cancellationToken)
    {
        Events.Write(new ExternalEvent(command.CommandId, $"hello from {nameof(SomeScopeExternalCommandHandler)}"));

        Console.WriteLine($"{nameof(SomeScopeExternalCommandHandler)} do something");

        // do nothing
        return Task.CompletedTask;
    }
}
