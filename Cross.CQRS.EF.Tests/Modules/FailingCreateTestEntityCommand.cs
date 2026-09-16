namespace Cross.CQRS.EF.Tests.Modules;

public class FailingCreateTestEntityCommand : Command
{
    public required string Name { get; set; }
}
