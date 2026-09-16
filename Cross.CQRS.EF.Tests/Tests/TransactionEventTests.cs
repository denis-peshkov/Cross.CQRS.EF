namespace Cross.CQRS.EF.Tests.Tests;

[TestFixture]
public class TransactionEventTests
{
    [Test]
    [Category(TestCategory.INTEGRATION)]
    public async Task GivenTransactionalBehavior_WhenCommandSucceeds_ThenPublishesEvents()
    {
        using var host = new SqlitePipelineHost(TransactionBehaviorEnum.TransactionalBehavior);
        var command = new CreateTestEntityCommand { Name = Guid.NewGuid().ToString("N") };

        await host.SendAsync(command);

        command.CommandId.Should().NotBe(Guid.Empty);
        host.PublishedEvents.Published.Should().ContainSingle(published => published.CommandId == command.CommandId);
    }

    [Test]
    [Category(TestCategory.INTEGRATION)]
    public async Task GivenTransactionalBehavior_WhenHandlerFails_ThenDoesNotPublishEvents()
    {
        using var host = new SqlitePipelineHost(TransactionBehaviorEnum.TransactionalBehavior);
        var command = new FailingCreateTestEntityCommand { Name = Guid.NewGuid().ToString("N") };

        var act = () => host.SendAsync(command);
        await act.Should().ThrowAsync<InvalidOperationException>();

        host.PublishedEvents.Published.Should().BeEmpty();
    }

    [Test]
    [Category(TestCategory.INTEGRATION)]
    public async Task GivenTransactionalBehavior_WhenCommitFails_ThenDoesNotPublishEvents()
    {
        using var host = new SqlitePipelineHost(TransactionBehaviorEnum.TransactionalBehavior);
        var command = new CreateTestEntityCommand { Name = Guid.NewGuid().ToString("N") };
        host.CommitFailure.FailNextCommit = true;

        var act = () => host.SendAsync(command);
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Simulated commit failure");

        host.PublishedEvents.Published.Should().BeEmpty();

        var entity = await host.ExecuteAsync(sp =>
            sp.GetRequiredService<TestDbContext>().TestEntities.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Name == command.Name));

        entity.Should().BeNull();
    }
}
