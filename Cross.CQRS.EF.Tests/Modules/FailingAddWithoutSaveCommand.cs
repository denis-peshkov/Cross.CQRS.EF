namespace Cross.CQRS.EF.Tests.Modules;

public class FailingAddWithoutSaveCommand : ICommand
{
    public Guid CommandId { get; }

    public required string Name { get; set; }
}
