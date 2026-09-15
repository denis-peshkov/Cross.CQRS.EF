namespace Cross.CQRS.EF.Licensing;

internal interface ILicenseProductInfo
{
    string Company { get; }
    string Product { get; }
    string Site { get; }
    string FullLoggerName => $"{Product} License";
    ProductTypeEnum[] Types { get; }
    string LicenseTypesErrMessage { get; }
}
