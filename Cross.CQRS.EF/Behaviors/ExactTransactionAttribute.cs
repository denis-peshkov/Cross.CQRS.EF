namespace Cross.CQRS.EF.Behaviors;

/// <summary>
/// Marker attribute to set exact transactional type like <see cref="UnifiedTransactionBehavior{TRequest, TResult}"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class ExactTransactionAttribute : Attribute
{
    public TransactionBehaviorEnum Value { get; }
    public IsolationLevel IsolationLevel { get; }

    public ExactTransactionAttribute(TransactionBehaviorEnum value, IsolationLevel isolationLevel = IsolationLevel.Serializable)
    {
        Value = value;
        IsolationLevel = isolationLevel;
    }
}
