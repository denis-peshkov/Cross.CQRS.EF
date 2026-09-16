namespace Cross.CQRS.EF.Tests.Modules;

public class CreateTestEntityCommand : Command
{
    public required string Name { get; set; }
}
