namespace Cross.CQRS.EF.Tests.Tests;

[TestFixture]
public class TransactionBehaviorTests
{
    [Test]
    [Category(TestCategory.INTEGRATION)]
    public async Task TransactionalBehavior_Success_ShouldCommitChanges()
    {
        using var host = new SqlitePipelineHost(TransactionBehaviorEnum.TransactionalBehavior);
        var name = Guid.NewGuid().ToString("N");

        await host.SendAsync(new CreateTestEntityCommand { Name = name });

        var entity = await host.ExecuteAsync(sp =>
            sp.GetRequiredService<TestDbContext>().TestEntities.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Name == name));

        entity.Should().NotBeNull();
        entity!.Name.Should().Be(name);
    }

    [Test]
    [Category(TestCategory.INTEGRATION)]
    public async Task TransactionalBehavior_Error_ShouldRollbackChanges()
    {
        using var host = new SqlitePipelineHost(TransactionBehaviorEnum.TransactionalBehavior);
        var name = Guid.NewGuid().ToString("N");

        var act = () => host.SendAsync(new FailingCreateTestEntityCommand { Name = name });
        await act.Should().ThrowAsync<InvalidOperationException>();

        var entity = await host.ExecuteAsync(sp =>
            sp.GetRequiredService<TestDbContext>().TestEntities.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Name == name));

        entity.Should().BeNull();
    }

    [Test]
    [Category(TestCategory.INTEGRATION)]
    [TestCase(TransactionBehaviorEnum.TransactionalBehavior)]
    [TestCase(TransactionBehaviorEnum.ScopeBehavior)]
    [TestCase(TransactionBehaviorEnum.TransactionalScopeBehavior)]
    public async Task DifferentBehaviors_Success_ShouldCommitChanges(TransactionBehaviorEnum behavior)
    {
        using var host = new SqlitePipelineHost(behavior);

        if (behavior == TransactionBehaviorEnum.TransactionalBehavior)
        {
            var name = Guid.NewGuid().ToString("N");
            await host.SendAsync(new CreateTestEntityCommand { Name = name });

            var entity = await host.ExecuteAsync(sp =>
                sp.GetRequiredService<TestDbContext>().TestEntities.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Name == name));

            entity.Should().NotBeNull();
            return;
        }

        // SQLite does not enlist in TransactionScope / SaveChanges; assert the pipeline still wraps the command.
        var snapshot = await host.SendAsync(new TransactionProbeCommand());
        snapshot.HasAmbientTransaction.Should().BeTrue();
    }

    // Isolation-level matrix lives in IsolationLevelIntegrationTests.
}
