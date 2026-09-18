namespace Cross.CQRS.EF.Tests.Modules.Events;

public sealed record TestEvent : CommandEvent
{
    public TestEvent(Guid commandId)
        : base(commandId)
    {
    }
}
