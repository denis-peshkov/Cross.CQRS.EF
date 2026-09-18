namespace SampleWebApp.Modules.Some.Handlers;

public sealed record SomeQuery : Query<IEnumerable<string>>
{
    public SomeQuery()
    {
    }
}
