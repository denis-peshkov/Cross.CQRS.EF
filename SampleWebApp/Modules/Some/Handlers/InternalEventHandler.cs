namespace SampleWebApp.Modules.Some.Handlers;

public class InternalEventHandler : Cross.CQRS.Events.EventHandler<InternalEvent>
{
    protected override Task HandleAsync(InternalEvent ev, CancellationToken cancellationToken)
    {
        Console.WriteLine($"{nameof(InternalEventHandler)} with message '{ev.Message}'");

        return Task.CompletedTask;
    }
}
