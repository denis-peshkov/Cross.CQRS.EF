namespace Cross.CQRS.EF.Tests.Tests;

[TestFixture]
public class TransactionEventTests : HandlerTestsBase
{
    private Mock<ICommandEventQueueWriter> _commandEventsMock;

    [OneTimeSetUp]
    public override void OneTimeSetUp()
    {
        base.OneTimeSetUp();

        _commandEventsMock = new Mock<ICommandEventQueueWriter>();
    }

    [Test]
    public async Task TransactionWithEvents_Success_ShouldPublishEvents()
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
        _commandEventsMock.Verify(x => x.Write(It.IsAny<ICommandEvent>()), Times.Once);
    }

    [Test]
    public async Task TransactionWithEvents_Failure_ShouldNotPublishEvents()
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

        _commandEventsMock.Verify(x => x.Write(It.IsAny<ICommandEvent>()), Times.Never);
    }
}
