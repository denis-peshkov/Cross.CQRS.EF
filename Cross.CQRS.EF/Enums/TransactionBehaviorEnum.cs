namespace Cross.CQRS.EF.Enums;

public enum TransactionBehaviorEnum
{
    NoBehavior = 0,

    TransactionalBehavior = 1,

    ScopeBehavior = 2,

    TransactionalScopeBehavior = 3,
}
