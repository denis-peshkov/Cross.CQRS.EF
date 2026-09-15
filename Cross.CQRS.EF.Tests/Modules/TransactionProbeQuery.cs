namespace Cross.CQRS.EF.Tests.Modules;

public class TransactionProbeQuery : IQuery<TransactionSnapshot>
{
    public Guid QueryId { get; }
}
