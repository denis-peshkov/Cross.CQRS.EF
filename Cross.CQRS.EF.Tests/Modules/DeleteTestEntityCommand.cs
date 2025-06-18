namespace Cross.CQRS.EF.Tests.Modules;

public class DeleteTestEntityCommand : ICommand
{
    public Guid CommandId { get; }
    public int Id { get; set; }
}
