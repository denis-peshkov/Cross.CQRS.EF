namespace Cross.CQRS.EF.Tests.Tests;

[TestFixture]
public class IsolationLevelIntegrationTests
{
    [Test]
    [Category(TestCategory.INTEGRATION)]
    [TestCase(IsolationLevel.ReadUncommitted)]
    [TestCase(IsolationLevel.ReadCommitted)]
    [TestCase(IsolationLevel.RepeatableRead)]
    [TestCase(IsolationLevel.Serializable)]
    public async Task GivenTransactionalBehavior_WhenIsolationConfigured_ThenBeginUsesLevelAndCommits(
        IsolationLevel isolationLevel)
    {
        using var host = new SqlitePipelineHost(TransactionBehaviorEnum.TransactionalBehavior, isolationLevel);
        var name = Guid.NewGuid().ToString("N");

        await host.SendAsync(new CreateTestEntityCommand { Name = name });

        host.IsolationCapture.LastStartedIsolationLevel.Should().Be(isolationLevel.ToDataIsolation());

        var entity = await host.ExecuteAsync(sp =>
            sp.GetRequiredService<TestDbContext>().TestEntities.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Name == name));

        entity.Should().NotBeNull();
        entity!.Name.Should().Be(name);
    }

    [Test]
    [Category(TestCategory.INTEGRATION)]
    [TestCase(IsolationLevel.ReadUncommitted)]
    [TestCase(IsolationLevel.ReadCommitted)]
    [TestCase(IsolationLevel.RepeatableRead)]
    [TestCase(IsolationLevel.Serializable)]
    public async Task GivenTransactionalBehavior_WhenIsolationConfiguredAndHandlerFails_ThenRollsBack(
        IsolationLevel isolationLevel)
    {
        using var host = new SqlitePipelineHost(TransactionBehaviorEnum.TransactionalBehavior, isolationLevel);
        var name = Guid.NewGuid().ToString("N");

        var act = () => host.SendAsync(new FailingCreateTestEntityCommand { Name = name });
        await act.Should().ThrowAsync<InvalidOperationException>();

        host.IsolationCapture.LastStartedIsolationLevel.Should().Be(isolationLevel.ToDataIsolation());

        var entity = await host.ExecuteAsync(sp =>
            sp.GetRequiredService<TestDbContext>().TestEntities.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Name == name));

        entity.Should().BeNull();
    }

    [Test]
    [Category(TestCategory.INTEGRATION)]
    [TestCase(IsolationLevel.ReadUncommitted)]
    [TestCase(IsolationLevel.ReadCommitted)]
    [TestCase(IsolationLevel.RepeatableRead)]
    [TestCase(IsolationLevel.Serializable)]
    public async Task GivenScopeBehavior_WhenIsolationConfigured_ThenAmbientUsesLevel(
        IsolationLevel isolationLevel)
    {
        using var host = new SqlitePipelineHost(TransactionBehaviorEnum.ScopeBehavior, isolationLevel);

        var snapshot = await host.SendAsync(new TransactionProbeCommand());

        snapshot.HasAmbientTransaction.Should().BeTrue();
        snapshot.AmbientIsolationLevel.Should().Be(isolationLevel);
    }

    [Test]
    [Category(TestCategory.INTEGRATION)]
    [TestCase(IsolationLevel.ReadUncommitted)]
    [TestCase(IsolationLevel.ReadCommitted)]
    [TestCase(IsolationLevel.RepeatableRead)]
    [TestCase(IsolationLevel.Serializable)]
    public async Task GivenTransactionalScopeBehavior_WhenIsolationConfigured_ThenAmbientUsesLevel(
        IsolationLevel isolationLevel)
    {
        using var host = new SqlitePipelineHost(TransactionBehaviorEnum.TransactionalScopeBehavior, isolationLevel);

        var snapshot = await host.SendAsync(new TransactionProbeCommand());

        snapshot.HasAmbientTransaction.Should().BeTrue();
        snapshot.AmbientIsolationLevel.Should().Be(isolationLevel);
    }

    [Test]
    [Category(TestCategory.INTEGRATION)]
    [TestCase(typeof(ExactTransactionReadUncommittedProbeCommand), IsolationLevel.ReadUncommitted)]
    [TestCase(typeof(ExactTransactionProbeCommand), IsolationLevel.ReadCommitted)]
    [TestCase(typeof(ExactTransactionRepeatableReadProbeCommand), IsolationLevel.RepeatableRead)]
    [TestCase(typeof(ExactTransactionSerializableProbeCommand), IsolationLevel.Serializable)]
    public async Task GivenExactTransactionOverride_WhenIsolationOnHandler_ThenBeginUsesThatLevel(
        Type commandType,
        IsolationLevel expectedIsolation)
    {
        // Global isolation differs so the assert proves ExactTransaction wins.
        using var host = new SqlitePipelineHost(TransactionBehaviorEnum.NoBehavior, IsolationLevel.Serializable);
        var command = (IRequest<TransactionSnapshot>)Activator.CreateInstance(commandType)!;

        await host.SendAsync(command);

        host.IsolationCapture.LastStartedIsolationLevel.Should().Be(expectedIsolation.ToDataIsolation());
    }
}
