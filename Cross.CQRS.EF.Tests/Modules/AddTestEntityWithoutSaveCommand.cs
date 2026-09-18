namespace Cross.CQRS.EF.Tests.Modules;

public sealed record AddTestEntityWithoutSaveCommand : Command
{
    public string Name { get; set; } = string.Empty;
}
