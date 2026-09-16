namespace Cross.CQRS.EF.Tests.Common;

internal sealed class IsolationCaptureInterceptor : DbTransactionInterceptor
{
    public System.Data.IsolationLevel? LastStartedIsolationLevel { get; private set; }

    public void Reset()
    {
        LastStartedIsolationLevel = null;
    }

    public override InterceptionResult<DbTransaction> TransactionStarting(
        DbConnection connection,
        TransactionStartingEventData eventData,
        InterceptionResult<DbTransaction> result)
    {
        LastStartedIsolationLevel = eventData.IsolationLevel;
        return result;
    }

    public override ValueTask<InterceptionResult<DbTransaction>> TransactionStartingAsync(
        DbConnection connection,
        TransactionStartingEventData eventData,
        InterceptionResult<DbTransaction> result,
        CancellationToken cancellationToken = default)
    {
        LastStartedIsolationLevel = eventData.IsolationLevel;
        return new ValueTask<InterceptionResult<DbTransaction>>(result);
    }
}
