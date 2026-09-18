namespace Cross.CQRS.EF.Tests.Modules;

public sealed record ExactTransactionProbeCommand : Command<TransactionSnapshot>
{
}
