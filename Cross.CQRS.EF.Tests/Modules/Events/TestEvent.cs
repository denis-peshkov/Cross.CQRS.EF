namespace Cross.CQRS.EF.Tests.Modules.Events;

public class TestEvent : ICommandEvent
{
    public Guid CommandId { get; init; }
}
