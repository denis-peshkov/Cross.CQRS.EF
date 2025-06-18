namespace Cross.CQRS.EF.Tests.Modules.Entities;

public class TestEvent : ICommandEvent
{
    public Guid CommandId { get; init; }
}
