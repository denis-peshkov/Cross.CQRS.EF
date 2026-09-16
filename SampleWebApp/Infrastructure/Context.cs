namespace SampleWebApp.Infrastructure;

public class Context : DbContext
{
    public Context(DbContextOptions<Context> options)
        : base(options)
    {
    }

    public DbSet<SampleEntity> SampleEntities { get; set; }
}
