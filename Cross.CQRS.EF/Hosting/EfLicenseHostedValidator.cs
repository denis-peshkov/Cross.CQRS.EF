namespace Cross.CQRS.EF.Hosting;

internal sealed class EfLicenseHostedValidator : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public EfLicenseHostedValidator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _serviceProvider.CheckLicense();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
