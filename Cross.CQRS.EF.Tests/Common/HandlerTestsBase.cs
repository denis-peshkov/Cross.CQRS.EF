namespace Cross.CQRS.EF.Tests.Common;

[TestFixture]
public abstract class HandlerTestsBase
{
    protected IConfiguration Configuration;
    protected Faker Faker;

    protected TestDbContext DbContext { get; private set; } = default!;
    private SqliteConnection _connection = default!;

    [OneTimeSetUp]
    public virtual void OneTimeSetUp()
    {
        Configuration = LoadConfiguration();
        Faker = new Faker();
    }

    [SetUp]
    public virtual void Setup()
    {
        // Открываем InMemory SQLite connection
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        // Строим контекст с этой connection
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite(_connection)
            .EnableSensitiveDataLogging()
            .Options;

        DbContext = (TestDbContext)Activator.CreateInstance(typeof(TestDbContext), options)!;

        // Важно: создать схему
        DbContext.Database.EnsureCreated();
    }

    [TearDown]
    public virtual void TearDown()
    {
        DbContext.Database.EnsureDeleted();
        DbContext?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
    }

    [OneTimeTearDown]
    public virtual void OneTimeTearDown()
    {
    }

    private static IConfiguration LoadConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Test.json", false, true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
        return builder.Build();
    }
}
