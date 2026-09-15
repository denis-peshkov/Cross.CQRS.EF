namespace Cross.CQRS.EF.Tests.Modules;

public class FailingCreateTestEntityCommand : ICommand
{
    public Guid CommandId { get; }

    public required string Name { get; set; }
}
