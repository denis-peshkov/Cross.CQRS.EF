namespace Cross.CQRS.EF.Tests.Modules;

public sealed class TransactionSnapshot
{
    public bool HasEfTransaction { get; init; }

    public bool HasAmbientTransaction { get; init; }

    public System.Data.IsolationLevel? EfIsolationLevel { get; init; }

    public IsolationLevel? AmbientIsolationLevel { get; init; }
}
