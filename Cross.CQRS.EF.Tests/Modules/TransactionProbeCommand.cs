namespace Cross.CQRS.EF.Tests.Modules;

public class TransactionProbeCommand : ICommand<TransactionSnapshot>
{
    public Guid CommandId { get; }
}
