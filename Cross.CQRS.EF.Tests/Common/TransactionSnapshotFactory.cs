namespace Cross.CQRS.EF.Tests.Common;

internal static class TransactionSnapshotFactory
{
    public static TransactionSnapshot Capture(DbContext dbContext)
    {
        System.Data.IsolationLevel? isolationLevel = null;
        if (dbContext.Database.CurrentTransaction != null)
        {
            isolationLevel = dbContext.Database.CurrentTransaction.GetDbTransaction().IsolationLevel;
        }

        return new TransactionSnapshot
        {
            HasEfTransaction = dbContext.Database.CurrentTransaction != null,
            HasAmbientTransaction = Transaction.Current != null,
            EfIsolationLevel = isolationLevel,
        };
    }
}
