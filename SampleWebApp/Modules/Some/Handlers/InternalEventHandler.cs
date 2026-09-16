namespace SampleWebApp.Modules.Some.Handlers;

public class InternalEventHandler : CommandEventHandler<InternalEvent>
{
    public InternalEventHandler(ILogger<InternalEventHandler> logger) : base(logger)
    {
    }

    protected override Task HandleAsync(InternalEvent ev, CancellationToken cancellationToken)
    {
        Console.WriteLine($"{nameof(InternalEventHandler)} with message '{ev.Message}'");

        return Task.CompletedTask;
    }
}
