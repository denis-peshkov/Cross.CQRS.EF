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
}
