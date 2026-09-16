namespace Cross.CQRS.EF.Options;

public class TransactionBehaviorOptions
{
    public TransactionBehaviorEnum Behavior { get; set; } = TransactionBehaviorEnum.TransactionalBehavior;
    public IsolationLevel IsolationLevel { get; set; } = IsolationLevel.Serializable;
}
