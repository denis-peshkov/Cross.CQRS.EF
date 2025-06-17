namespace Cross.CQRS.EF.Options;

public class TransactionBehaviorOptions
{
    public TransactionBehaviorEnum Behavior { get; set; } = TransactionBehaviorEnum.TransactionalScopeBehavior;
    public IsolationLevel IsolationLevel { get; set; } = IsolationLevel.ReadCommitted;
}
