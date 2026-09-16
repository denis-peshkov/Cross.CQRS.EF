namespace Cross.CQRS.EF.Tests.Modules;

public class FailingAddWithoutSaveCommand : Command
{
    public required string Name { get; set; }
}
