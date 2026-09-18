namespace SampleWebApp.Modules.Some.Handlers;

public sealed record SomeScopeExternalCommand : Command
{
    public SomeScopeExternalCommand()
    {
    }

    public string Name { get; set; }
}
