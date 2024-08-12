using Cross.CQRS.EF.Enums;

namespace Cross.CQRS.EF;

public static class CqrsRegistrationSyntaxExtensions
{
    /// <summary>
    /// Registers required services from the specified assemblies to the
    /// specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="transactionBehavior"></param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static CqrsRegistrationSyntax AddEntityFrameworkIntegration<TDbContext>(this CqrsRegistrationSyntax syntax, TransactionBehaviorEnum transactionBehavior = TransactionBehaviorEnum.TransactionalBehavior)
        where TDbContext : DbContext
    {
        // Registration order is important, it works like ASP.NET Core middleware
        // Behaviors registered earlier will be executed earlier
        switch (transactionBehavior)
        {
            case TransactionBehaviorEnum.TransactionalBehavior:
                syntax.Behaviors.AddBehavior(typeof(TransactionalBehavior<,>), order: 10);
                break;
            case TransactionBehaviorEnum.ScopeBehavior:
                syntax.Behaviors.AddBehavior(typeof(ScopeBehavior<,>), order: 10);
                break;
            case TransactionBehaviorEnum.TransactionalScopeBehavior:
                syntax.Behaviors.AddBehavior(typeof(TransactionalScopeBehavior<,>), order: 10);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(transactionBehavior), transactionBehavior, null);
        }

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
