namespace Cross.CQRS.EF.Tests.Modules;

public sealed record FailingAddWithoutSaveCommand : Command
{
    public string Name { get; set; } = string.Empty;
}
