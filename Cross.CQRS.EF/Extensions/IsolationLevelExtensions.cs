namespace Cross.CQRS.EF.Extensions;

public static class IsolationLevelExtensions
{
    public static System.Data.IsolationLevel ToDataIsolation(this System.Transactions.IsolationLevel txLevel)
    {
        return txLevel switch
        {
            System.Transactions.IsolationLevel.Serializable     => System.Data.IsolationLevel.Serializable,
            System.Transactions.IsolationLevel.RepeatableRead   => System.Data.IsolationLevel.RepeatableRead,
            System.Transactions.IsolationLevel.ReadCommitted    => System.Data.IsolationLevel.ReadCommitted,
            System.Transactions.IsolationLevel.ReadUncommitted  => System.Data.IsolationLevel.ReadUncommitted,
            System.Transactions.IsolationLevel.Snapshot         => System.Data.IsolationLevel.Snapshot,
            System.Transactions.IsolationLevel.Chaos            => System.Data.IsolationLevel.Chaos,
            System.Transactions.IsolationLevel.Unspecified      => System.Data.IsolationLevel.Unspecified,
            _ => throw new ArgumentOutOfRangeException(nameof(txLevel), txLevel, null)
        };
    }

    public static System.Transactions.IsolationLevel ToTxIsolation(this System.Data.IsolationLevel dataLevel)
    {
        return dataLevel switch
        {
            System.Data.IsolationLevel.Serializable    => System.Transactions.IsolationLevel.Serializable,
            System.Data.IsolationLevel.RepeatableRead  => System.Transactions.IsolationLevel.RepeatableRead,
            System.Data.IsolationLevel.ReadCommitted   => System.Transactions.IsolationLevel.ReadCommitted,
            System.Data.IsolationLevel.ReadUncommitted => System.Transactions.IsolationLevel.ReadUncommitted,
            System.Data.IsolationLevel.Snapshot        => System.Transactions.IsolationLevel.Snapshot,
            System.Data.IsolationLevel.Chaos           => System.Transactions.IsolationLevel.Chaos,
            System.Data.IsolationLevel.Unspecified     => System.Transactions.IsolationLevel.Unspecified,
            _ => throw new ArgumentOutOfRangeException(nameof(dataLevel), dataLevel, null)
        };
    }
}