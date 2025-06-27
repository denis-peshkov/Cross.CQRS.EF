namespace Cross.CQRS.EF.Extensions;

public static class CqrsRegistrationSyntaxExtensions
{
    /// <summary>
    /// Registers required Entity Framework integration services from the specified assemblies to the
    /// specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TDbContext">The type of the database context.</typeparam>
    /// <param name="syntax">The CQRS registration syntax instance.</param>
    /// <param name="transactionBehavior">The transaction behavior to use. Defaults to TransactionalBehavior.</param>
    /// <param name="isolationLevel">The transaction isolation level to use. Defaults to Serializable.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <remarks>
    /// This method performs the following registrations:
    /// <list type="bullet">
    /// <item><description>Registers transaction behavior options</description></item>
    /// <item><description>Adds UnifiedTransactionBehavior with order 10</description></item>
    /// <item><description>Registers all implementations of IQueryableFilter from specified assemblies</description></item>
    /// <item><description>Registers DbContextProvider for the specified DbContext type</description></item>
    /// </list>
    /// </remarks>
    public static CqrsRegistrationSyntax AddEntityFrameworkIntegration<TDbContext>(
        this CqrsRegistrationSyntax syntax,
        TransactionBehaviorEnum transactionBehavior = TransactionBehaviorEnum.TransactionalBehavior,
        IsolationLevel isolationLevel = IsolationLevel.Serializable)
        where TDbContext : DbContext
    {
        var options = new TransactionBehaviorOptions
        {
            Behavior = transactionBehavior,
            IsolationLevel = isolationLevel,
        };
        syntax.Services.AddSingleton(options);

        // Registration order is important, it works like ASP.NET Core middleware
        // Behaviors registered earlier will be executed earlier
        syntax.Behaviors.AddBehavior(typeof(UnifiedTransactionBehavior<,>), order: 10);

        syntax.Services.TryAddScoped<IDbContextProvider, DbContextProvider<TDbContext>>();

        return syntax;
    }
}
