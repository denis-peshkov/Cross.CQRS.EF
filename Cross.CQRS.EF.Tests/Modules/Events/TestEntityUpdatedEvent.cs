namespace Cross.CQRS.EF.Tests.Modules.Events;

public class TestEntityUpdatedEvent : ICommandEvent
{
    public Guid CommandId { get; init; }
    public int Id { get; set; }
    public string Name { get; set; }
}
