namespace Cross.CQRS.EF.Tests.Modules;

public class UpdateTestEntityCommand : Command
{
    public int Id { get; set; }
    public required string Name { get; set; }
}
