namespace Cross.CQRS.EF.Tests.Modules;

public class AddTestEntityWithoutSaveCommand : Command
{
    public required string Name { get; set; }
}
