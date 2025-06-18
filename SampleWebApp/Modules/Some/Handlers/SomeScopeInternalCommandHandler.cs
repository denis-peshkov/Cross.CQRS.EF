namespace SampleWebApp.Modules.Some.Handlers;

public class SomeScopeInternalCommandHandler : CommandHandler<SomeScopeExternalCommand>
{

    public SomeScopeInternalCommandHandler(ICommandEventQueueWriter commandEvents, ILogger<SomeScopeExternalCommandHandler> logger)
        : base(commandEvents, logger)
    {
    }

    protected override Task HandleAsync(SomeScopeExternalCommand command, CancellationToken cancellationToken)
    {
        CommandEvents.Write(new InternalEvent(command.CommandId, $"hello from {nameof(SomeScopeInternalCommandHandler)}"));

        Console.WriteLine($"{nameof(SomeScopeInternalCommandHandler)} do something");

        // do nothing
        return Task.CompletedTask;
    }
}
