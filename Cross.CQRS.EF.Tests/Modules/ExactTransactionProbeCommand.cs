namespace Cross.CQRS.EF.Tests.Modules;

public class ExactTransactionProbeCommand : ICommand<TransactionSnapshot>
{
    public Guid CommandId { get; }
}
