namespace Cross.CQRS.EF.Tests.Modules;

public sealed record CreateTestEntityCommand : Command
{
    public string Name { get; set; } = string.Empty;
}
