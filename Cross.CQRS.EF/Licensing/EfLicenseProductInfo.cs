namespace Cross.CQRS.EF.Licensing;

/// <summary>
/// Product metadata for this package. Requires <see cref="ProductTypeEnum.Cross_CQRS_EF"/> in the JWT type claim.
/// </summary>
internal sealed class EfLicenseProductInfo : ILicenseProductInfo
{
    public string Company => "Peshkov software";
    public string Product => "Cross.CQRS.EF";
    public string Site => "https://peshkov.biz";
    public ProductTypeEnum[] Types { get; } = { ProductTypeEnum.Cross_CQRS_EF };
    public string LicenseTypesErrMessage => $"Your {Company} license does not include {Product} (expected {ProductTypeEnum.Cross_CQRS_EF} in the license type claim).";
}
