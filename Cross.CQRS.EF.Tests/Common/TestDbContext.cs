namespace Cross.CQRS.EF.Tests.Common;

public class TestDbContext : DbContext
{
    public DbSet<TestEntity> TestEntities { get; set; }

    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {
    }

    public async Task<IDbContextTransaction> SafeBeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken = default)
    {
        if (System.Transactions.Transaction.Current != null || Database.CurrentTransaction != null)
        {
            // В режиме тестов или SQLite → skip, чтобы не было ошибки
            Console.WriteLine("Skipping BeginTransactionAsync due to AmbientTransaction.");
            return new NoopTransaction();
        }

        return await Database.BeginTransactionAsync(isolationLevel.ToDataIsolation(), cancellationToken);
    }

    // Конфигурация модели (опционально — можно без этого)
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TestEntity>(entity =>
        {
            entity.ToTable("TestEntities");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.BalanceAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.CreatedOn).IsRequired();
        });
    }

    public class NoopTransaction : IDbContextTransaction
    {
        public Guid TransactionId => Guid.Empty;
        public void Commit() { }
        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Rollback() { }
        public Task RollbackAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Dispose() { }
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
        public IDbContextTransaction GetDbTransaction() => this;
    }
}
