namespace Cross.CQRS.EF.Services;

internal class DbContextProvider<TDbContext> : IDbContextProvider
    where TDbContext : DbContext
{
    private readonly TDbContext _dbContext;

    public DbContextProvider(TDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public DbContext Get() => _dbContext;
}
