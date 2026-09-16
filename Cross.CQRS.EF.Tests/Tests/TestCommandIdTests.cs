namespace Cross.CQRS.EF.Tests.Tests;

[TestFixture]
public class TestCommandIdTests
{
    [Test]
    [Category(TestCategory.UNIT)]
    public void GivenDeleteTestEntityCommand_WhenConstructed_ThenCommandIdIsNotEmpty()
    {
        var command = new DeleteTestEntityCommand { Id = 1 };

        command.CommandId.Should().NotBe(Guid.Empty);
    }

    [Test]
    [Category(TestCategory.UNIT)]
    public void GivenCreateTestEntityCommand_WhenConstructed_ThenCommandIdIsNotEmpty()
    {
        var command = new CreateTestEntityCommand { Name = "n" };

        command.CommandId.Should().NotBe(Guid.Empty);
    }

    [Test]
    [Category(TestCategory.UNIT)]
    public void GivenUpdateTestEntityCommand_WhenConstructed_ThenCommandIdIsNotEmpty()
    {
        var command = new UpdateTestEntityCommand { Id = 1, Name = "n" };

        command.CommandId.Should().NotBe(Guid.Empty);
    }

    [Test]
    [Category(TestCategory.UNIT)]
    public void GivenFailingCreateTestEntityCommand_WhenConstructed_ThenCommandIdIsNotEmpty()
    {
        var command = new FailingCreateTestEntityCommand { Name = "n" };

        command.CommandId.Should().NotBe(Guid.Empty);
    }

    [Test]
    [Category(TestCategory.UNIT)]
    public void GivenFailingAddWithoutSaveCommand_WhenConstructed_ThenCommandIdIsNotEmpty()
    {
        var command = new FailingAddWithoutSaveCommand { Name = "n" };

        command.CommandId.Should().NotBe(Guid.Empty);
    }

    [Test]
    [Category(TestCategory.UNIT)]
    public void GivenFailingMutateTrackedEntitiesCommand_WhenConstructed_ThenCommandIdIsNotEmpty()
    {
        var command = new FailingMutateTrackedEntitiesCommand();

        command.CommandId.Should().NotBe(Guid.Empty);
    }

    [Test]
    [Category(TestCategory.UNIT)]
    public void GivenAddTestEntityWithoutSaveCommand_WhenConstructed_ThenCommandIdIsNotEmpty()
    {
        var command = new AddTestEntityWithoutSaveCommand { Name = "n" };

        command.CommandId.Should().NotBe(Guid.Empty);
    }

    [Test]
    [Category(TestCategory.UNIT)]
    [TestCase(typeof(ExactTransactionReadUncommittedProbeCommand))]
    [TestCase(typeof(ExactTransactionRepeatableReadProbeCommand))]
    [TestCase(typeof(ExactTransactionSerializableProbeCommand))]
    public void GivenExactTransactionIsolationProbeCommand_WhenConstructed_ThenCommandIdIsNotEmpty(Type commandType)
    {
        var command = (Command<TransactionSnapshot>)Activator.CreateInstance(commandType)!;

        command.CommandId.Should().NotBe(Guid.Empty);
    }

    [Test]
    [Category(TestCategory.UNIT)]
    public void GivenExactTransactionProbeCommand_WhenConstructed_ThenCommandIdIsNotEmpty()
    {
        new ExactTransactionProbeCommand().CommandId.Should().NotBe(Guid.Empty);
    }

    [Test]
    [Category(TestCategory.UNIT)]
    public void GivenTransactionProbeCommand_WhenConstructed_ThenCommandIdIsNotEmpty()
    {
        new TransactionProbeCommand().CommandId.Should().NotBe(Guid.Empty);
    }

    [Test]
    [Category(TestCategory.UNIT)]
    public void GivenTransactionProbeQuery_WhenConstructed_ThenQueryIdIsNotEmpty()
    {
        new TransactionProbeQuery().QueryId.Should().NotBe(Guid.Empty);
    }
}
