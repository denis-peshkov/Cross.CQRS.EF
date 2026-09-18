namespace Cross.CQRS.EF.Tests.Modules;

public sealed record TransactionProbeCommand : Command<TransactionSnapshot>
{
}
