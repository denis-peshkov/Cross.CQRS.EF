var builder = WebApplication.CreateBuilder(args);

// Add services to the container. Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.TryAddScoped<Context>();

//MediatR
builder.Services
    .AddCQRS(cfg => cfg.RegisterFromAssemblies(typeof(Program).Assembly))
    .AddEntityFrameworkIntegration<Context>(TransactionBehaviorEnum.ScopeBehavior);

// builder.Services
//     .AddCQRS(cfg =>
//     {
//         cfg.RegisterFromAssemblies(typeof(Program).Assembly);
//         cfg.LicenseKey = "eyJhbGciOiJSUzI1NiIsImtpZCI6IlBlc2hrb3ZTb2Z0d2FyZUxpY2Vuc2VLZXkvZjhlN2Q2YzViNGEzOTI4MTcwNmY1ZTRkM2MyYjFhMDkiLCJ0eXAiOiJKV1QifQ.eyJzdWJfaWQiOiJiNzdhMjg2ZC0xMWU5LTQ2M2EtODdmMS01ZDU2MzA5ZTMxZTkiLCJ1c2VyX2lkIjoiNjViYmQxNmItZDYwMy00NDYzLWIxNjktMzEyNDcxYzcwYTc0IiwiaWF0IjoxNzcxMTk5NTE3LCJuYmYiOjE3NzExOTk1MTcsImV4cCI6MTgwMjczNTUxNywiZWRpdGlvbiI6IkNvbW11bml0eSIsInR5cGUiOiJDcm9zc19DUVJTIiwiaXNzIjoiaHR0cHM6Ly9wZXNoa292LmJpeiIsImF1ZCI6IlBlc2hrb3Ygc29mdHdhcmUifQ.HYdE8jUeHg76PQGM-hc1Sdmf3eawZ0EsW9rvth239TeA1JTiAkSI-J-kF-IFq2MYsmS3OAfp0VpWM9sk3AXiWW0jg-0Vgvcex7LT5ZEEiHP5R1GdjoFXQvgmzDNZO6EmqLy9B0qiQLTMTTSOmnJ4NRJMfJHTsqxYOckV67URdERLbjfrcwqH7Xnmqhsvun3jB2pAspnr4TxHUD4xePNdqnEA4WvUiY5JhxWgFkub31SEpxsvAfoi8-bx8rQ7cjIaCEWvjitzpCkdWzSg416GRmNP_CicYClq3nN66tgo3wdJsbHY4PlkzO3NcuY9ADOTZWywwylFKbx6FgN9PxcPsg";
//     })
//     .AddEntityFrameworkIntegration<Context>(TransactionBehaviorEnum.ScopeBehavior);

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
