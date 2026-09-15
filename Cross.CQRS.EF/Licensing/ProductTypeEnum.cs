namespace Cross.CQRS.Licensing;

[SuppressMessage("ReSharper", "InconsistentNaming")]
internal enum ProductTypeEnum
{
    /// <summary>Product line Cross.CQRS (claim <c>type</c> in the license JWT).</summary>
    Cross_CQRS = 1,

    /// <summary>Product line Cross.CQRS.EF (claim <c>type</c> in the license JWT).</summary>
    Cross_CQRS_EF = 2,
}
