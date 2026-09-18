namespace Cross.CQRS.EF.Tests.Modules;

public sealed record DeleteTestEntityCommand : Command
{
    public int Id { get; set; }
}
