namespace Cross.CQRS.EF;

public static class CqrsRegistrationSyntaxExtensions
{
    public static CqrsRegistrationSyntax AddEntityFrameworkIntegration<TDbContext>(this CqrsRegistrationSyntax syntax)
        where TDbContext : DbContext
    {
        // Registration order is important, it works like ASP.NET Core middleware
        // Behaviors registered earlier will be executed earlier
        syntax.Behaviors.AddBehavior(typeof(TransactionalBehavior<,>), order: 10);

        // Filters
        syntax.Services.Scan(scan =>
            scan.FromAssemblies(syntax.Assemblies.Distinct())
                .AddClasses(classes => classes.AssignableTo(typeof(IQueryableFilter<,>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

        syntax.Services.TryAddScoped<IDbContextProvider, DbContextProvider<TDbContext>>();

        return syntax;
    }
}
