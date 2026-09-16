namespace SampleWebApp.Modules.Some.Handlers;

public class ExternalEventHandler : CommandEventHandler<ExternalEvent>
{
    public ExternalEventHandler(ILogger<ExternalEventHandler> logger) : base(logger)
    {
    }

    protected override Task HandleAsync(ExternalEvent ev, CancellationToken cancellationToken)
    {
        Console.WriteLine($"{nameof(ExternalEventHandler)} with message '{ev.Message}'");

        return Task.CompletedTask;
    }
}
