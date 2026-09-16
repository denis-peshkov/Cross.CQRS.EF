#if NET6_0
namespace System.Runtime.CompilerServices
#else
namespace Cross.CQRS.EF.Tests.Polyfills
#endif
{
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property,
        AllowMultiple = false,
        Inherited = false)]
    internal sealed class RequiredMemberAttribute : Attribute
    {
    }
}
