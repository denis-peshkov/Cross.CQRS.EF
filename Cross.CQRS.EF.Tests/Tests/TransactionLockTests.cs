namespace Cross.CQRS.EF.Tests.Tests;

public class TransactionLockTests : HandlerTestsBase
{
    private TestDbContext _dbContext1;
    private TestDbContext _dbContext2;
    private SqliteConnection _keepAlive;
    private string _connectionString;
    private string _databasePath;
    private Mock<ICommandEventQueueWriter> _commandEventsMock;
    private Mock<ILogger<UpdateTestEntityHandler>> _loggerMock;

    [OneTimeSetUp]
    public override void OneTimeSetUp()
    {
        base.OneTimeSetUp();

        _commandEventsMock = new Mock<ICommandEventQueueWriter>();
        _loggerMock = new Mock<ILogger<UpdateTestEntityHandler>>();
    }

    [SetUp]
    public new void Setup()
    {
        // In-memory SQLite ignores WAL and takes a table lock after uncommitted SaveChanges,
        // so the observer connection cannot read the last committed snapshot. A temp file + WAL
        // keeps a dedicated connection per context and lets the ReadCommitted handshake proceed.
        _databasePath = Path.Combine(Path.GetTempPath(), $"cross-cqrs-ef-lock-{Guid.NewGuid():N}.db");
        _connectionString = $"Data Source={_databasePath}";
        _keepAlive = new SqliteConnection(_connectionString);
        _keepAlive.Open();
        using (var journalMode = _keepAlive.CreateCommand())
        {
            journalMode.CommandText = "PRAGMA journal_mode=WAL;";
            journalMode.ExecuteNonQuery();
        }

        _dbContext1 = CreateContext();
        _dbContext2 = CreateContext();
        _dbContext1.Database.EnsureCreated();
    }

    [TearDown]
    public new void TearDown()
    {
        try
        {
            _dbContext1?.Dispose();
            _dbContext2?.Dispose();
            _keepAlive?.Dispose();
        }
        finally
        {
            DeleteDatabaseFiles();
        }
    }

    private TestDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite(_connectionString)
            .EnableSensitiveDataLogging()
            .Options;

        return new TestDbContext(options);
    }

    [Test]
    public async Task ReadCommitted_Update_ShouldNotBlockRead()
    {
        var entity = await CreateTestEntity();
        var updateCommand = new UpdateTestEntityCommand { Id = entity.Id, Name = Faker.Company.CompanyName() };
        var completed = false;
        var writeSaved = CreateSignal();
        var readFinished = CreateSignal();

        var updateTask = Task.Run(async () =>
        {
            using var transaction = await _dbContext1.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted.ToDataIsolation());
            try
            {
                var handler = new UpdateTestEntityHandler(_commandEventsMock.Object, _loggerMock.Object, _dbContext1);
                await handler.Handle(updateCommand, CancellationToken.None);
                writeSaved.TrySetResult();
                await readFinished.Task;
                await transaction.CommitAsync();
                completed = true;
            }
            catch
            {
                writeSaved.TrySetResult();
                await transaction.RollbackAsync();
                throw;
            }
        });

        try
        {
            await writeSaved.Task;

            var readEntity = await _dbContext2.TestEntities.AsNoTracking().FirstOrDefaultAsync(x => x.Id == entity.Id);
            readEntity.Should().NotBeNull();
            readEntity!.Name.Should().Be(entity.Name);
        }
        finally
        {
            readFinished.TrySetResult();
        }

        await updateTask;
        completed.Should().BeTrue();

        var updatedEntity = await _dbContext2.TestEntities.AsNoTracking().FirstOrDefaultAsync(x => x.Id == entity.Id);
        updatedEntity.Name.Should().Be(updateCommand.Name);
    }

    [Test]
    [Ignore("Not finished yet")]
    public async Task RepeatableRead_Update_ShouldBlockRead()
    {
        var entity = await CreateTestEntity();
        var updateCommand = new UpdateTestEntityCommand { Id = entity.Id, Name = Faker.Company.CompanyName() };
        var completed = false;
        var writeSaved = CreateSignal();
        var allowCommit = CreateSignal();

        var updateTask = Task.Run(async () =>
        {
            using var transaction = await _dbContext1.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead.ToDataIsolation());
            try
            {
                var handler = new UpdateTestEntityHandler(_commandEventsMock.Object, _loggerMock.Object, _dbContext1);
                await handler.Handle(updateCommand, CancellationToken.None);
                writeSaved.TrySetResult();
                await allowCommit.Task;
                await transaction.CommitAsync();
                completed = true;
            }
            catch
            {
                writeSaved.TrySetResult();
                await transaction.RollbackAsync();
                throw;
            }
        });

        try
        {
            await writeSaved.Task;

            var readTask = Task.Run(async () =>
            {
                using var transaction = await _dbContext2.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead.ToDataIsolation());
                return await _dbContext2.TestEntities.AsNoTracking().FirstOrDefaultAsync(x => x.Id == entity.Id);
            });

            var timeoutTask = Task.Delay(5000);
            var completedTask = await Task.WhenAny(readTask, timeoutTask);
            completedTask.Should().Be(timeoutTask, "Read operation should be blocked");
        }
        finally
        {
            allowCommit.TrySetResult();
        }

        await updateTask;
        completed.Should().BeTrue();
    }

    [Test]
    [Ignore("Not finished yet")]
    public async Task Serializable_ConcurrentUpdates_ShouldBlockSecondUpdate()
    {
        var entity = await CreateTestEntity();
        var updateCommand1 = new UpdateTestEntityCommand { Id = entity.Id, Name = Faker.Company.CompanyName() };
        var updateCommand2 = new UpdateTestEntityCommand { Id = entity.Id, Name = Faker.Company.CompanyName() };
        var secondUpdateException = false;
        var firstWriteSaved = CreateSignal();
        var allowFirstCommit = CreateSignal();

        var updateTask1 = Task.Run(async () =>
        {
            await using var transaction = await _dbContext1.SafeBeginTransactionAsync(IsolationLevel.Serializable);
            try
            {
                var handler = new UpdateTestEntityHandler(_commandEventsMock.Object, _loggerMock.Object, _dbContext1);
                await handler.Handle(updateCommand1, CancellationToken.None);
                firstWriteSaved.TrySetResult();
                await allowFirstCommit.Task;
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                firstWriteSaved.TrySetResult();
                await transaction.RollbackAsync();
                throw;
            }
        });

        await firstWriteSaved.Task;

        var updateTask2 = Task.Run(async () =>
        {
            try
            {
                await using var transaction = await _dbContext2.SafeBeginTransactionAsync(IsolationLevel.Serializable);
                var handler = new UpdateTestEntityHandler(_commandEventsMock.Object, _loggerMock.Object, _dbContext2);
                await handler.Handle(updateCommand2, CancellationToken.None);
                await transaction.CommitAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                secondUpdateException = true;
                return false;
            }
            catch (DbUpdateException)
            {
                secondUpdateException = true;
                return false;
            }
        });

        try
        {
            await updateTask2;
        }
        finally
        {
            allowFirstCommit.TrySetResult();
        }

        await updateTask1;

        secondUpdateException.Should().BeTrue("Second update should fail due to serialization conflict");

        var finalEntity = await _dbContext1.TestEntities.AsNoTracking().FirstAsync(e => e.Id == entity.Id);
        finalEntity.Name.Should().Be(updateCommand1.Name);
    }

    [Test]
    [Ignore("Not finished yet")]
    public async Task Serializable_ConcurrentUpdates_ShouldBlockSecondUpdate_v2()
    {
        var entity = await CreateTestEntity();
        var updateCommand1 = new UpdateTestEntityCommand { Id = entity.Id, Name = Faker.Company.CompanyName() };
        var updateCommand2 = new UpdateTestEntityCommand { Id = entity.Id, Name = Faker.Company.CompanyName() };
        var secondUpdateException = false;
        var firstWriteSaved = CreateSignal();
        var allowFirstCommit = CreateSignal();

        var updateTask1 = Task.Run(async () =>
        {
            using var scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Serializable },
                TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                var handler = new UpdateTestEntityHandler(_commandEventsMock.Object, _loggerMock.Object, _dbContext1);
                await handler.Handle(updateCommand1, CancellationToken.None);
                firstWriteSaved.TrySetResult();
                await allowFirstCommit.Task;
                scope.Complete();
                return true;
            }
            catch
            {
                firstWriteSaved.TrySetResult();
                throw;
            }
        });

        await firstWriteSaved.Task;

        var updateTask2 = Task.Run(async () =>
        {
            try
            {
                using var scope = new TransactionScope(
                    TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Serializable },
                    TransactionScopeAsyncFlowOption.Enabled);
                var handler = new UpdateTestEntityHandler(_commandEventsMock.Object, _loggerMock.Object, _dbContext2);
                await handler.Handle(updateCommand2, CancellationToken.None);
                scope.Complete();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                secondUpdateException = true;
                return false;
            }
            catch (DbUpdateException)
            {
                secondUpdateException = true;
                return false;
            }
        });

        try
        {
            await updateTask2;
        }
        finally
        {
            allowFirstCommit.TrySetResult();
        }

        await updateTask1;

        secondUpdateException.Should().BeTrue("Second update should fail due to serialization conflict");

        var finalEntity = await _dbContext1.TestEntities.AsNoTracking().FirstAsync(e => e.Id == entity.Id);
        finalEntity.Name.Should().Be(updateCommand1.Name);
    }


    private static TaskCompletionSource CreateSignal()
        => new(TaskCreationOptions.RunContinuationsAsynchronously);

    private async Task<TestEntity> CreateTestEntity()
    {
        var command = new CreateTestEntityCommand { Name = Faker.Company.CompanyName() };
        var loggerMock = new Mock<ILogger<CreateTestEntityHandler>>();
        var handler = new CreateTestEntityHandler(_commandEventsMock.Object, loggerMock.Object, _dbContext1);
        await handler.Handle(command, CancellationToken.None);
        return await _dbContext1.TestEntities.FirstOrDefaultAsync(x => x.Name == command.Name);
    }
}
