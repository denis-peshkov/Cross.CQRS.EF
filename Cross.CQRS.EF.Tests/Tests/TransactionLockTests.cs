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

    private static readonly TimeSpan ConcurrentWaitTimeout = TimeSpan.FromSeconds(3);

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
        _databasePath = Path.Combine(Path.GetTempPath(), $"cross-cqrs-ef-lock-{Guid.NewGuid():N}.db");
        // Default Timeout (seconds) bounds how long SQLite waits on locks before failing the command.
        _connectionString = $"Data Source={_databasePath};Pooling=False;Default Timeout=1";
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

    [Test]
    [Category(TestCategory.INTEGRATION)]
    public async Task GivenReadCommittedWriteInProgress_WhenObserverReads_ThenSeesLastCommittedSnapshotAsync()
    {
        // WAL lets the observer read the last committed snapshot while the writer holds an open transaction.
        OpenContexts(useWal: true, busyTimeoutMilliseconds: 5000);
        await EnsureCreatedAsync();

        var entity = await CreateTestEntityAsync();
        var originalName = entity.Name;
        var updateCommand = new UpdateTestEntityCommand { Id = entity.Id, Name = Faker.Company.CompanyName() };
        var writeSaved = CreateSignal();
        var readFinished = CreateSignal();
        var completed = false;

        var updateTask = Task.Run(async () =>
        {
            await using var transaction = await _dbContext1.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted.ToDataIsolation());
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
    [Category(TestCategory.INTEGRATION)]
    public async Task GivenExclusiveWriteLock_WhenSecondConnectionReads_ThenReadStaysBlockedUntilCommitAsync()
    {
        // DELETE journal: an uncommitted writer takes a reserved lock; readers block (unlike WAL).
        OpenContexts(useWal: false, busyTimeoutMilliseconds: 10_000);
        await EnsureCreatedAsync();

        var entity = await CreateTestEntityAsync();
        var updateCommand = new UpdateTestEntityCommand { Id = entity.Id, Name = Faker.Company.CompanyName() };
        var writeSaved = CreateSignal();
        var allowCommit = CreateSignal();
        var completed = false;

        var updateTask = Task.Run(async () =>
        {
            await using var transaction = await _dbContext1.Database.BeginTransactionAsync(IsolationLevel.Serializable.ToDataIsolation());
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
                await using var transaction = await _dbContext2.Database.BeginTransactionAsync(IsolationLevel.Serializable.ToDataIsolation());
                return await _dbContext2.TestEntities.AsNoTracking().FirstOrDefaultAsync(x => x.Id == entity.Id);
            });

            var timeoutTask = Task.Delay(ConcurrentWaitTimeout);
            var completedTask = await Task.WhenAny(readTask, timeoutTask);
            completedTask.Should().Be(timeoutTask, "read should still be waiting on the writer lock");
            readTask.IsCompleted.Should().BeFalse();
        }
        finally
        {
            allowCommit.TrySetResult();
        }

        await updateTask.WaitAsync(ConcurrentWaitTimeout);
        completed.Should().BeTrue();
    }

    [Test]
    [Category(TestCategory.INTEGRATION)]
    public async Task GivenExclusiveWriteLock_WhenSecondConnectionWrites_ThenSecondSaveChangesFailsAsync()
    {
        // busy_timeout=0 → second writer fails immediately with SQLITE_BUSY instead of waiting.
        OpenContexts(useWal: false, busyTimeoutMilliseconds: 0);
        await EnsureCreatedAsync();
        _dbContext2.Database.SetCommandTimeout(TimeSpan.FromSeconds(1));

        var entity = await CreateTestEntityAsync();
        var updateCommand1 = new UpdateTestEntityCommand { Id = entity.Id, Name = Faker.Company.CompanyName() };
        var updateCommand2 = new UpdateTestEntityCommand { Id = entity.Id, Name = Faker.Company.CompanyName() };
        var firstWriteSaved = CreateSignal();
        var allowFirstCommit = CreateSignal();
        Exception? secondUpdateException = null;

        var updateTask1 = Task.Run(async () =>
        {
            await using var transaction = await _dbContext1.Database.BeginTransactionAsync(IsolationLevel.Serializable.ToDataIsolation());
            try
            {
                var handler = new UpdateTestEntityHandler(_commandEventsMock.Object, _loggerMock.Object, _dbContext1);
                await handler.Handle(updateCommand1, CancellationToken.None);
                firstWriteSaved.TrySetResult();
                await allowFirstCommit.Task;
                await transaction.CommitAsync();
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
                await using var transaction = await _dbContext2.Database.BeginTransactionAsync(IsolationLevel.Serializable.ToDataIsolation());
                var handler = new UpdateTestEntityHandler(_commandEventsMock.Object, _loggerMock.Object, _dbContext2);
                await handler.Handle(updateCommand2, CancellationToken.None);
                await transaction.CommitAsync();
            }
            catch (Exception exception)
            {
                secondUpdateException = exception;
            }
        });

        try
        {
            // Default Timeout=1s on the connection → blocked write must fail (or finish) quickly.
            await updateTask2.WaitAsync(TimeSpan.FromSeconds(5));
        }
        finally
        {
            allowFirstCommit.TrySetResult();
        }

        await updateTask1.WaitAsync(ConcurrentWaitTimeout);

        secondUpdateException.Should().NotBeNull("second writer should fail while the first holds the write lock");
        IsLockConflict(secondUpdateException!).Should().BeTrue($"unexpected exception: {secondUpdateException}");

        // Reload on a fresh connection after both writers finished — context2 may be poisoned by the failed write.
        await using var verifyConnection = OpenConnection(busyTimeoutMilliseconds: 5000);
        await using var verifyContext = CreateContext(verifyConnection);
        var finalEntity = await verifyContext.TestEntities.AsNoTracking().FirstAsync(e => e.Id == entity.Id);
        finalEntity.Name.Should().Be(updateCommand1.Name);
    }

    private void OpenContexts(bool useWal, int busyTimeoutMilliseconds)
    {
        _connection1 = OpenConnection(busyTimeoutMilliseconds);
        if (useWal)
        {
            using var journalMode = _connection1.CreateCommand();
            journalMode.CommandText = "PRAGMA journal_mode=WAL;";
            journalMode.ExecuteNonQuery();
        }

        _connection2 = OpenConnection(busyTimeoutMilliseconds);
        _dbContext1 = CreateContext(_connection1);
        _dbContext2 = CreateContext(_connection2);
    }

    private SqliteConnection OpenConnection(int busyTimeoutMilliseconds)
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();
        using var busyTimeout = connection.CreateCommand();
        busyTimeout.CommandText = $"PRAGMA busy_timeout={busyTimeoutMilliseconds};";
        busyTimeout.ExecuteNonQuery();
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

    private async Task EnsureCreatedAsync()
    {
        await _dbContext1.Database.EnsureCreatedAsync();
    }

    private static TaskCompletionSource CreateSignal()
        => new(TaskCreationOptions.RunContinuationsAsynchronously);

    private static bool IsLockConflict(Exception exception)
    {
        for (var current = exception; current != null; current = current.InnerException)
        {
            if (current is TimeoutException)
            {
                return true;
            }

            if (current is SqliteException sqliteException &&
                (sqliteException.SqliteErrorCode == 5 ||
                 sqliteException.SqliteExtendedErrorCode == 5 ||
                 sqliteException.SqliteErrorCode == 6 ||
                 sqliteException.Message.Contains("database is locked", StringComparison.OrdinalIgnoreCase) ||
                 sqliteException.Message.Contains("busy", StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            if (current is DbUpdateException dbUpdateException &&
                dbUpdateException.InnerException != null &&
                IsLockConflict(dbUpdateException.InnerException))
            {
                return true;
            }
        }

        return false;
    }

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

    private async Task<TestEntity> CreateTestEntityAsync()
    {
        var command = new CreateTestEntityCommand { Name = Faker.Company.CompanyName() };
        var loggerMock = new Mock<ILogger<CreateTestEntityHandler>>();
        var handler = new CreateTestEntityHandler(_commandEventsMock.Object, loggerMock.Object, _dbContext1);
        await handler.Handle(command, CancellationToken.None);
        return await _dbContext1.TestEntities.AsNoTracking().FirstAsync(x => x.Name == command.Name);
    }
}
