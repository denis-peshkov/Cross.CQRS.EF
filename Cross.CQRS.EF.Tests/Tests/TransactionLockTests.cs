namespace Cross.CQRS.EF.Tests.Tests;

public class TransactionLockTests : HandlerTestsBase
{
    private TestDbContext _dbContext1;
    private TestDbContext _dbContext2;
    private SqliteConnection _keepAlive;
    private string _connectionString;
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
        // Shared in-memory DB with a dedicated connection per context (SQLite connections are not thread-safe).
        _connectionString = $"Data Source=file:{Guid.NewGuid():N}?mode=memory&cache=shared";
        _keepAlive = new SqliteConnection(_connectionString);
        _keepAlive.Open();

        _dbContext1 = CreateContext();
        _dbContext2 = CreateContext();
        _dbContext1.Database.EnsureCreated();
    }

    [TearDown]
    public new void TearDown()
    {
        _dbContext1?.Dispose();
        _dbContext2?.Dispose();
        _keepAlive?.Dispose();
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
        // Arrange
        var entity = await CreateTestEntity();
        var updateCommand = new UpdateTestEntityCommand { Id = entity.Id, Name = Faker.Company.CompanyName() };
        var completed = false;

        // Act
        var updateTask = Task.Run(async () =>
        {
            using var transaction = await _dbContext1.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted.ToDataIsolation());
            try
            {
                var handler = new UpdateTestEntityHandler(_commandEventsMock.Object, _loggerMock.Object, _dbContext1);
                await handler.Handle(updateCommand, CancellationToken.None);
                await Task.Delay(2000); // Имитация длительной операции
                await transaction.CommitAsync();
                completed = true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });

        await Task.Delay(500); // Даем время начать транзакцию обновления

        // Assert - проверяем, что чтение не блокируется
        var readEntity = await _dbContext2.TestEntities.AsNoTracking().FirstOrDefaultAsync(x => x.Id == entity.Id);
        readEntity.Should().NotBeNull();
        readEntity.Name.Should().Be(entity.Name); // Должно вернуть старое значение

        await updateTask;
        completed.Should().BeTrue();

        // Проверяем, что изменения применились после завершения транзакции
        var updatedEntity = await _dbContext2.TestEntities.AsNoTracking().FirstOrDefaultAsync(x => x.Id == entity.Id);
        updatedEntity.Name.Should().Be(updateCommand.Name);
    }

    [Test]
    [Ignore("Not finished yet")]
    public async Task RepeatableRead_Update_ShouldBlockRead()
    {
        // Arrange
        var entity = await CreateTestEntity();
        var updateCommand = new UpdateTestEntityCommand { Id = entity.Id, Name = Faker.Company.CompanyName() };
        var completed = false;

        // Act
        var updateTask = Task.Run(async () =>
        {
            using var transaction = await _dbContext1.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead.ToDataIsolation());
            try
            {
                var handler = new UpdateTestEntityHandler(_commandEventsMock.Object, _loggerMock.Object, _dbContext1);
                await handler.Handle(updateCommand, CancellationToken.None);
                await Task.Delay(2000); // Имитация длительной операции
                await transaction.CommitAsync();
                completed = true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });

        await Task.Delay(500); // Даем время начать транзакцию обновления

        // Assert - проверяем, что чтение блокируется
        var readTask = Task.Run(async () =>
        {
            using var transaction = await _dbContext2.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead.ToDataIsolation());
            return await _dbContext2.TestEntities.AsNoTracking().FirstOrDefaultAsync(x => x.Id == entity.Id);
        });

        var timeoutTask = Task.Delay(5000);
        var completedTask = await Task.WhenAny(readTask, timeoutTask);
        completedTask.Should().Be(timeoutTask, "Read operation should be blocked");

        await updateTask;
        completed.Should().BeTrue();
    }

    [Test]
    [Ignore("Not finished yet")]
    public async Task Serializable_ConcurrentUpdates_ShouldBlockSecondUpdate()
    {
        // Arrange
        var entity = await CreateTestEntity();
        var updateCommand1 = new UpdateTestEntityCommand { Id = entity.Id, Name = Faker.Company.CompanyName() };
        var updateCommand2 = new UpdateTestEntityCommand { Id = entity.Id, Name = Faker.Company.CompanyName() };
        var secondUpdateException = false;

        // Act
        var updateTask1 = Task.Run(async () =>
        {
            await using var transaction = await _dbContext1.SafeBeginTransactionAsync(IsolationLevel.Serializable);
            try
            {
                var handler = new UpdateTestEntityHandler(_commandEventsMock.Object, _loggerMock.Object, _dbContext1);
                await handler.Handle(updateCommand1, CancellationToken.None);
                await Task.Delay(2000); // Имитация длительной операции
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });

        // Ждем, пока первая транзакция начнется
        await Task.Delay(500);

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

        // Assert
        await updateTask1;
        await updateTask2;

        secondUpdateException.Should().BeTrue("Second update should fail due to serialization conflict");

        // Проверяем, что применились изменения только от первой транзакции
        var finalEntity = await _dbContext1.TestEntities.AsNoTracking().FirstAsync(e => e.Id == entity.Id);
        finalEntity.Name.Should().Be(updateCommand1.Name);
    }

    [Test]
    [Ignore("Not finished yet")]
    public async Task Serializable_ConcurrentUpdates_ShouldBlockSecondUpdate_v2()
    {
        // Arrange
        var entity = await CreateTestEntity();
        var updateCommand1 = new UpdateTestEntityCommand { Id = entity.Id, Name = Faker.Company.CompanyName() };
        var updateCommand2 = new UpdateTestEntityCommand { Id = entity.Id, Name = Faker.Company.CompanyName() };
        var secondUpdateException = false;

        // Act
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
                await Task.Delay(2000); // Имитация длительной операции
                scope.Complete();
                return true;
            }
            catch
            {
                throw;
            }
        });

        await Task.Delay(500);

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

        // Assert
        await updateTask1;
        await updateTask2;

        secondUpdateException.Should().BeTrue("Second update should fail due to serialization conflict");

        // Проверяем, что применились изменения только от первой транзакции
        var finalEntity = await _dbContext1.TestEntities.AsNoTracking().FirstAsync(e => e.Id == entity.Id);
        finalEntity.Name.Should().Be(updateCommand1.Name);
    }


    private async Task<TestEntity> CreateTestEntity()
    {
        var command = new CreateTestEntityCommand { Name = Faker.Company.CompanyName() };
        var loggerMock = new Mock<ILogger<CreateTestEntityHandler>>();
        var handler = new CreateTestEntityHandler(_commandEventsMock.Object, loggerMock.Object, _dbContext1);
        await handler.Handle(command, CancellationToken.None);
        return await _dbContext1.TestEntities.FirstOrDefaultAsync(x => x.Name == command.Name);
    }
}
