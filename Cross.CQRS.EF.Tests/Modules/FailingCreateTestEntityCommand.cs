namespace Cross.CQRS.EF.Tests.Modules;

public sealed record FailingCreateTestEntityCommand : Command
{
    public string Name { get; set; } = string.Empty;
}
