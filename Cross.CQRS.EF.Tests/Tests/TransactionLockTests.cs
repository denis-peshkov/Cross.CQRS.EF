namespace Cross.CQRS.EF.Tests.Tests;

public class TransactionLockTests : HandlerTestsBase
{
    private TestDbContext _dbContext1;
    private TestDbContext _dbContext2;
    private SqliteConnection _connection1;
    private SqliteConnection _connection2;
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
        // In-memory SQLite ignores WAL and takes a table lock after uncommitted SaveChanges.
        // A temp file + WAL lets the observer connection read the last committed snapshot.
        // Pin an open connection per context so SaveChanges stays in the writer's transaction
        // instead of auto-committing through a pooled connection.
        _databasePath = Path.Combine(Path.GetTempPath(), $"cross-cqrs-ef-lock-{Guid.NewGuid():N}.db");
        _connectionString = $"Data Source={_databasePath};Pooling=False";
        _connection1 = OpenConnection();
        using (var journalMode = _connection1.CreateCommand())
        {
            journalMode.CommandText = "PRAGMA journal_mode=WAL;";
            journalMode.ExecuteNonQuery();
        }

        _connection2 = OpenConnection();
        _dbContext1 = CreateContext(_connection1);
        _dbContext2 = CreateContext(_connection2);
        _dbContext1.Database.EnsureCreated();
    }

    [TearDown]
    public new void TearDown()
    {
        try
        {
            _dbContext1?.Dispose();
            _dbContext2?.Dispose();
            _connection1?.Dispose();
            _connection2?.Dispose();
        }
        finally
        {
            DeleteDatabaseFiles();
        }
    }

    private SqliteConnection OpenConnection()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();
        return connection;
    }

    private static TestDbContext CreateContext(SqliteConnection connection)
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite(connection)
            .EnableSensitiveDataLogging()
            .Options;

        return new TestDbContext(options);
    }

    [Test]
    public async Task ReadCommitted_Update_ShouldNotBlockRead()
    {
        var entity = await CreateTestEntity();
        var originalName = entity.Name;
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
            await writeSaved.Task.WaitAsync(ConcurrentWaitTimeout);

            var readEntity = await _dbContext2.TestEntities.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == entity.Id)
                .WaitAsync(ConcurrentWaitTimeout);
            readEntity.Should().NotBeNull();
            readEntity!.Name.Should().Be(originalName);
        }
        finally
        {
            readFinished.TrySetResult();
        }

        await updateTask.WaitAsync(ConcurrentWaitTimeout);
        completed.Should().BeTrue();

        var updatedEntity = await _dbContext2.TestEntities.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == entity.Id)
            .WaitAsync(ConcurrentWaitTimeout);
        updatedEntity.Should().NotBeNull();
        updatedEntity!.Name.Should().Be(updateCommand.Name);
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
            await writeSaved.Task.WaitAsync(ConcurrentWaitTimeout);

            var readTask = Task.Run(async () =>
            {
                using var transaction = await _dbContext2.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead.ToDataIsolation());
                return await _dbContext2.TestEntities.AsNoTracking().FirstOrDefaultAsync(x => x.Id == entity.Id);
            });

            var timeoutTask = Task.Delay(ConcurrentWaitTimeout);
            var completedTask = await Task.WhenAny(readTask, timeoutTask);
            completedTask.Should().Be(timeoutTask, "Read operation should be blocked");
        }
        finally
        {
            allowCommit.TrySetResult();
        }

        await updateTask.WaitAsync(ConcurrentWaitTimeout);
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

        await firstWriteSaved.Task.WaitAsync(ConcurrentWaitTimeout);

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
            await updateTask2.WaitAsync(ConcurrentWaitTimeout);
        }
        finally
        {
            allowFirstCommit.TrySetResult();
        }

        await updateTask1.WaitAsync(ConcurrentWaitTimeout);

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

        await firstWriteSaved.Task.WaitAsync(ConcurrentWaitTimeout);

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
            await updateTask2.WaitAsync(ConcurrentWaitTimeout);
        }
        finally
        {
            allowFirstCommit.TrySetResult();
        }

        await updateTask1.WaitAsync(ConcurrentWaitTimeout);

        secondUpdateException.Should().BeTrue("Second update should fail due to serialization conflict");

        var finalEntity = await _dbContext1.TestEntities.AsNoTracking().FirstAsync(e => e.Id == entity.Id);
        finalEntity.Name.Should().Be(updateCommand1.Name);
    }


    private static readonly TimeSpan ConcurrentWaitTimeout = TimeSpan.FromSeconds(5);

    private static TaskCompletionSource CreateSignal()
        => new(TaskCreationOptions.RunContinuationsAsynchronously);

    private void DeleteDatabaseFiles()
    {
        if (string.IsNullOrEmpty(_databasePath))
        {
            return;
        }

        foreach (var path in new[] { _databasePath, _databasePath + "-wal", _databasePath + "-shm" })
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (IOException)
            {
                // Best-effort cleanup; leftover temp files are reclaimed by the OS.
            }
        }
    }

    private async Task<TestEntity> CreateTestEntity()
    {
        var command = new CreateTestEntityCommand { Name = Faker.Company.CompanyName() };
        var loggerMock = new Mock<ILogger<CreateTestEntityHandler>>();
        var handler = new CreateTestEntityHandler(_commandEventsMock.Object, loggerMock.Object, _dbContext1);
        await handler.Handle(command, CancellationToken.None);
        return await _dbContext1.TestEntities.AsNoTracking().FirstAsync(x => x.Name == command.Name);
    }
}
