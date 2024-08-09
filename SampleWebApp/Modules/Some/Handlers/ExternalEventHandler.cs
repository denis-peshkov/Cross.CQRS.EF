namespace SampleWebApp.Modules.Some.Handlers;

public class ExternalEventHandler : Cross.CQRS.Events.EventHandler<ExternalEvent>
{
    protected override Task HandleAsync(ExternalEvent ev, CancellationToken cancellationToken)
    {
        Console.WriteLine($"{nameof(ExternalEventHandler)} with message '{ev.Message}'");

        return Task.CompletedTask;
    }
}
