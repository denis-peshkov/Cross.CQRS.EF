#if NET6_0

namespace Cross.CQRS.EF.Tests.Polyfills;

[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property,
    AllowMultiple = false,
    Inherited = false)]
internal sealed class RequiredMemberAttribute : Attribute
{
}

#endif
