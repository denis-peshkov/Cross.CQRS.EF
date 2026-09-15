namespace Cross.CQRS.EF.Tests.Licensing;

[TestFixture]
public class LicensingRegistrationTests
{
    [Test]
    public void GivenAddEntityFrameworkIntegration_WhenRegistered_ThenRegistersEfLicenseProductInfoAndPipelineSlot()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services
            .AddCQRS(cfg => cfg.RegisterFromAssemblyContaining<LicensingProbeRequest>())
            .AddEntityFrameworkIntegration<LicensingProbeDbContext>();

        services.Should().Contain(d =>
            d.ServiceType.FullName == "Cross.CQRS.Licensing.ILicenseProductInfo" &&
            d.ImplementationType != null &&
            d.ImplementationType.Name == "EfLicenseProductInfo");

        services.Should().Contain(d =>
            d.ImplementationType != null &&
            d.ImplementationType.Name == "EfLicenseCheckBehavior`2");

        services.Should().Contain(d =>
            d.ServiceType == typeof(IHostedService) &&
            d.ImplementationType != null &&
            d.ImplementationType.Name == "EfLicenseHostedValidator");
    }

    [Test]
    public void GivenAddEntityFrameworkIntegration_WhenProviderBuilt_ThenCheckLicenseViaCoreDoesNotThrow()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDbContext<LicensingProbeDbContext>(o => o.UseInMemoryDatabase(Guid.NewGuid().ToString()));

        services
            .AddCQRS(cfg => cfg.RegisterFromAssemblyContaining<LicensingProbeRequest>())
            .AddEntityFrameworkIntegration<LicensingProbeDbContext>();

        using var provider = services.BuildServiceProvider();
        var hosted = provider.GetServices<IHostedService>().OfType<object>()
            .First(s => s.GetType().Name == "EfLicenseHostedValidator");

        var start = hosted.GetType().GetMethod("StartAsync");
        start.Should().NotBeNull();
        var act = () => start!.Invoke(hosted, new object[] { CancellationToken.None });
        act.Should().NotThrow();
    }

    private sealed class LicensingProbeRequest : IRequest<string>
    {
    }

    private sealed class LicensingProbeRequestHandler : IRequestHandler<LicensingProbeRequest, string>
    {
        public Task<string> Handle(LicensingProbeRequest request, CancellationToken cancellationToken)
            => Task.FromResult("ok");
    }

    private sealed class LicensingProbeDbContext : DbContext
    {
        public LicensingProbeDbContext(DbContextOptions<LicensingProbeDbContext> options)
            : base(options)
        {
        }
    }
}
