#if NET6_0
namespace System.Runtime.CompilerServices
#else
namespace Cross.CQRS.EF.Tests.Polyfills
#endif
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    internal sealed class CompilerFeatureRequiredAttribute : Attribute
    {
        public CompilerFeatureRequiredAttribute(string featureName)
        {
            FeatureName = featureName;
        }

        public string FeatureName { get; }

        public bool IsOptional { get; set; }

        public const string RefStructs = nameof(RefStructs);

        public const string RequiredMembers = nameof(RequiredMembers);
    }
}
