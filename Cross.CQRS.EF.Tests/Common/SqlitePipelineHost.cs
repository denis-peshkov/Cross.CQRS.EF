namespace Cross.CQRS.EF.Tests.Common;

internal sealed class SqlitePipelineHost : IDisposable
{
    private readonly SqliteConnection _keepAlive;
    private readonly ServiceProvider _provider;

    public IsolationCaptureInterceptor IsolationCapture { get; }

    public CommitFailureInterceptor CommitFailure { get; }

    public RecordingTestEventHandler PublishedEvents { get; }

    public SqlitePipelineHost(
        TransactionBehaviorEnum behavior = TransactionBehaviorEnum.TransactionalBehavior,
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        IsolationCapture = new IsolationCaptureInterceptor();
        CommitFailure = new CommitFailureInterceptor();
        PublishedEvents = new RecordingTestEventHandler();
        _keepAlive = new SqliteConnection($"Data Source=file:{Guid.NewGuid():N}?mode=memory&cache=shared");
        _keepAlive.Open();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(PublishedEvents);
        services.AddSingleton<INotificationHandler<TestEvent>>(PublishedEvents);
        services.AddDbContext<TestDbContext>(options =>
        {
            options.UseSqlite(_keepAlive.ConnectionString);
            options.AddInterceptors(IsolationCapture, CommitFailure);
        });
        services
            .AddCQRS(cfg => cfg.RegisterFromAssemblyContaining<CreateTestEntityCommand>())
            .AddEntityFrameworkIntegration<TestDbContext>(behavior, isolationLevel);

        _provider = services.BuildServiceProvider();

        using var scope = _provider.CreateScope();
        scope.ServiceProvider.GetRequiredService<TestDbContext>().Database.EnsureCreated();
        IsolationCapture.Reset();
    }

    public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        using var scope = _provider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        return await mediator.Send(request, cancellationToken);
    }

    public async Task<TResult> ExecuteAsync<TResult>(Func<IServiceProvider, Task<TResult>> action)
    {
        using var scope = _provider.CreateScope();
        return await action(scope.ServiceProvider);
    }

    public void Dispose()
    {
        _provider.Dispose();
        _keepAlive.Dispose();
    }
}
