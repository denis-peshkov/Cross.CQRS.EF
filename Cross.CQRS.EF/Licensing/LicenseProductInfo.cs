namespace Cross.CQRS.EF.Licensing;

internal class LicenseProductInfo : ILicenseProductInfo
{
    public string Company => "Peshkov software";
    public string Product => "Cross.CQRS.EF";
    public string Site => "https://peshkov.biz";
    public ProductTypeEnum[] Types { get; } = { ProductTypeEnum.Cross_CQRS_EF };
    public string LicenseTypesErrMessage => $"Your {Company} license does not include {Product} (expected {ProductTypeEnum.Cross_CQRS_EF} in the license type claim).";
}
