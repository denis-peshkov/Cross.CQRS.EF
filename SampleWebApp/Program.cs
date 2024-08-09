var builder = WebApplication.CreateBuilder(args);

// Add services to the container. Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.TryAddScoped<Context>();

//MediatR
builder.Services
    .AddCQRS(typeof(Program).Assembly)
    .AddEntityFrameworkIntegration<Context>(TransactionBehaviorEnum.ScopeBehavior);

// services.AddDbContext<MyDbContext>(options => options.UseSqlServer(...));
// services.AddScoped<IGenericRepository<MyModel>, GenericRepository<MyModel>>();
// services.AddScoped<IMyDbContext, MyDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/somescope", (IMediator mediator ) =>
    {
        var forecast = mediator.Send(new SomeScopeExternalCommand());
        return forecast;
    })
    .WithName("RunSomeScope");

app.Run();
