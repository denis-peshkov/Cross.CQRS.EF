namespace Cross.CQRS.EF.Tests.Modules.Events;

public sealed record TestEntityUpdatedEvent : CommandEvent
{
    public TestEntityUpdatedEvent(Guid commandId)
        : base(commandId)
    {
    }

    public int Id { get; set; }

    public string Name { get; set; }
}
