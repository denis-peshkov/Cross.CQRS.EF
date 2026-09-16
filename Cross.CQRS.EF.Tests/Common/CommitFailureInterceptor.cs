namespace Cross.CQRS.EF.Tests.Common;

internal sealed class CommitFailureInterceptor : DbTransactionInterceptor
{
    public bool FailNextCommit { get; set; }

    public override InterceptionResult TransactionCommitting(
        DbTransaction transaction,
        TransactionEventData eventData,
        InterceptionResult result)
    {
        ThrowIfRequested();
        return result;
    }

    public override ValueTask<InterceptionResult> TransactionCommittingAsync(
        DbTransaction transaction,
        TransactionEventData eventData,
        InterceptionResult result,
        CancellationToken cancellationToken = default)
    {
        ThrowIfRequested();
        return new ValueTask<InterceptionResult>(result);
    }

    private void ThrowIfRequested()
    {
        if (!FailNextCommit)
        {
            return;
        }

        FailNextCommit = false;
        throw new InvalidOperationException("Simulated commit failure");
    }
}
