var builder = WebApplication.CreateBuilder(args);

// Add services to the container. Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

var sqliteKeepAlive = new SqliteConnection("Data Source=file:SampleWebApp?mode=memory&cache=shared");
sqliteKeepAlive.Open();
builder.Services.AddSingleton(sqliteKeepAlive);
builder.Services.AddDbContext<Context>(options =>
    options.UseSqlite(sqliteKeepAlive.ConnectionString));

//MediatR
builder.Services
    .AddCQRS(cfg =>
    {
        cfg.RegisterFromAssemblies(typeof(Program).Assembly);
        cfg.LicenseKey = "YOUR_LICENSE_KEY";
    })
    .AddEntityFrameworkIntegration<Context>(TransactionBehaviorEnum.ScopeBehavior);

// services.AddDbContext<MyDbContext>(options => options.UseSqlServer(...));
// services.AddScoped<IGenericRepository<MyModel>, GenericRepository<MyModel>>();
// services.AddScoped<IMyDbContext, MyDbContext>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<Context>();
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/somescope", async (IMediator mediator, Context dbContext, CancellationToken cancellationToken) =>
    {
        var name = Guid.NewGuid().ToString("N");
        await mediator.Send(new SomeScopeExternalCommand { Name = name }, cancellationToken);
        var entity = await dbContext.SampleEntities.AsNoTracking()
            .SingleAsync(x => x.Name == name, cancellationToken);
        return Results.Ok(entity);
    })
    .WithName("CreateSomeScope");

app.MapGet("/somescope", async (Context dbContext, CancellationToken cancellationToken) =>
        await dbContext.SampleEntities.AsNoTracking().ToListAsync(cancellationToken))
    .WithName("ListSomeScope");

app.Run();
