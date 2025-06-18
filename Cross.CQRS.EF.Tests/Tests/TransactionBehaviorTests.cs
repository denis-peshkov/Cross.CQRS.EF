namespace Cross.CQRS.EF.Tests.Tests;

[TestFixture]
public class TransactionBehaviorTests : HandlerTestsBase
{
    private Mock<ICommandEventQueueWriter> _commandEventsMock;

    [OneTimeSetUp]
    public override void OneTimeSetUp()
    {
        base.OneTimeSetUp();

        _commandEventsMock = new Mock<ICommandEventQueueWriter>();
    }

    [Test]
    public async Task TransactionalBehavior_Success_ShouldCommitChanges()
    {
        // Arrange
        var command = new CreateTestEntityCommand { Name = Faker.Company.CompanyName() };
        var loggerMock = new Mock<ILogger<CreateTestEntityHandler>>();

        // Act
        await DbContext.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
        {
            await using var transaction = await DbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            try
            {
                await new CreateTestEntityHandler(_commandEventsMock.Object, loggerMock.Object, DbContext)
                    .Handle(command, CancellationToken.None);
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });

        // Assert
        var entity = await DbContext.TestEntities.FirstOrDefaultAsync(x => x.Name == command.Name);
        entity.Should().NotBeNull();
        entity.Name.Should().NotBeNull();
        entity.Name.Should().Be(command.Name);
    }

    [Test]
    public async Task TransactionalBehavior_Error_ShouldRollbackChanges()
    {
        // Arrange
        var command = new CreateTestEntityCommand { Name = Faker.Company.CompanyName() };
        var loggerMock = new Mock<ILogger<FailingCreateTestEntityHandler>>();

        // Act & Assert
        await DbContext.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
        {
            await using var transaction = await DbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            try
            {
                var handler = new FailingCreateTestEntityHandler(_commandEventsMock.Object, loggerMock.Object, DbContext);
                var act = () => handler.Handle(command, CancellationToken.None);
                await act.Should().ThrowAsync<InvalidOperationException>();

                await transaction.RollbackAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });

        var entity = await DbContext.TestEntities.FirstOrDefaultAsync(x => x.Name == command.Name);
        entity.Should().BeNull();
    }

    [Test]
    [TestCase(TransactionBehaviorEnum.TransactionalBehavior)]
    [TestCase(TransactionBehaviorEnum.ScopeBehavior)]
    [TestCase(TransactionBehaviorEnum.TransactionalScopeBehavior)]
    public async Task DifferentBehaviors_Success_ShouldCommitChanges(TransactionBehaviorEnum behavior)
    {
        // Arrange
        var command = new CreateTestEntityCommand { Name = Faker.Company.CompanyName() };
        var loggerMock = new Mock<ILogger<CreateTestEntityHandler>>();

        // Act
        await DbContext.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
        {
            await using var transaction = await DbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            try
            {
                await new CreateTestEntityHandler(_commandEventsMock.Object, loggerMock.Object, DbContext)
                    .Handle(command, CancellationToken.None);
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });

        // Assert
        var entity = await DbContext.TestEntities.FirstOrDefaultAsync(x => x.Name == command.Name);
        entity.Should().NotBeNull();
    }

    [Test]
    [TestCase(IsolationLevel.ReadUncommitted)]
    [TestCase(IsolationLevel.ReadCommitted)]
    [TestCase(IsolationLevel.RepeatableRead)]
    [TestCase(IsolationLevel.Serializable)]
    public async Task DifferentIsolationLevels_Success_ShouldCommitChanges(IsolationLevel isolationLevel)
    {
        // Arrange
        var command = new CreateTestEntityCommand { Name = Faker.Company.CompanyName() };
        var loggerMock = new Mock<ILogger<CreateTestEntityHandler>>();

        // Act
        await DbContext.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
        {
            await using var transaction = await DbContext.Database.BeginTransactionAsync(isolationLevel);
            try
            {
                await new CreateTestEntityHandler(_commandEventsMock.Object, loggerMock.Object, DbContext)
                    .Handle(command, CancellationToken.None);
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });

        // Assert
        var entity = await DbContext.TestEntities.FirstOrDefaultAsync(x => x.Name == command.Name);
        entity.Should().NotBeNull();
    }
}
