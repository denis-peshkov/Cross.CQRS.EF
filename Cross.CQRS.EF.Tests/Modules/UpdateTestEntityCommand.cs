namespace Cross.CQRS.EF.Tests.Modules;

public class UpdateTestEntityCommand : ICommand
{
    public Guid CommandId { get; }
    public int Id { get; set; }
    public required string Name { get; set; }
}
