namespace SampleWebApp.Modules.Some.Handlers;

public class SomeScopeInternalCommandHandler : CommandHandler<SomeScopeExternalCommand>
{

    public SomeScopeInternalCommandHandler(IEventQueueWriter eventQueueWriter)
        : base(eventQueueWriter)
    {
    }

    protected override Task HandleAsync(SomeScopeExternalCommand command, CancellationToken cancellationToken)
    {
        Events.Write(new InternalEvent(command.CommandId, $"hello from {nameof(SomeScopeInternalCommandHandler)}"));

        Console.WriteLine($"{nameof(SomeScopeInternalCommandHandler)} do something");

        // do nothing
        return Task.CompletedTask;
    }
}
