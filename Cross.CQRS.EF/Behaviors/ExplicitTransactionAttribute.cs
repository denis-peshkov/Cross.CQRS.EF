namespace Cross.CQRS.EF.Behaviors;

/// <summary>
/// Marker attribute to disable <see cref="TransactionalBehavior{TRequest, TResult}"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class ExplicitTransactionAttribute : Attribute
{
}
