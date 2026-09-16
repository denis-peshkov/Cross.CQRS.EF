namespace Cross.CQRS.EF.Tests.Tests;

[TestFixture]
public class UnifiedTransactionBehaviorTests
{
    [Test]
    [Category(TestCategory.UNIT)]
    public void GivenAddEntityFrameworkIntegrationDefaults_WhenOptionsResolved_ThenUsesTransactionalBehaviorAndSerializable()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services
            .AddCQRS(cfg => cfg.RegisterFromAssemblyContaining<CreateTestEntityCommand>())
            .AddEntityFrameworkIntegration<TestDbContext>();

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<TransactionBehaviorOptions>>().Value;

        options.Behavior.Should().Be(TransactionBehaviorEnum.TransactionalBehavior);
        options.IsolationLevel.Should().Be(IsolationLevel.Serializable);
    }

    [Test]
    [Category(TestCategory.UNIT)]
    public void GivenTransactionBehaviorOptionsDefaults_WhenConstructed_ThenMatchExtensionParameters()
    {
        var options = new TransactionBehaviorOptions();

        options.Behavior.Should().Be(TransactionBehaviorEnum.TransactionalBehavior);
        options.IsolationLevel.Should().Be(IsolationLevel.Serializable);
    }

    [Test]
    [Category(TestCategory.UNIT)]
    public void GivenAddEntityFrameworkIntegration_WhenBehaviorAndIsolationPassed_ThenOptionsMatchArguments()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services
            .AddCQRS(cfg => cfg.RegisterFromAssemblyContaining<CreateTestEntityCommand>())
            .AddEntityFrameworkIntegration<TestDbContext>(
                TransactionBehaviorEnum.NoBehavior,
                IsolationLevel.Snapshot);

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<TransactionBehaviorOptions>>().Value;

        options.Behavior.Should().Be(TransactionBehaviorEnum.NoBehavior);
        options.IsolationLevel.Should().Be(IsolationLevel.Snapshot);
    }

    [Test]
    [Category(TestCategory.INTEGRATION)]
    public async Task GivenTransactionalBehavior_WhenCommandSucceeds_ThenCommitsChangesAsync()
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
    public async Task GivenScopeBehavior_WhenCommandRuns_ThenHandlerSeesAmbientTransactionAsync()
    {
        // SQLite does not enlist in TransactionScope; assert the wrapper still flows an ambient transaction.
        using var host = new SqlitePipelineHost(TransactionBehaviorEnum.ScopeBehavior);

        var snapshot = await host.SendAsync(new TransactionProbeCommand());

        snapshot.HasAmbientTransaction.Should().BeTrue();
    }

    [Test]
    [Category(TestCategory.INTEGRATION)]
    public async Task GivenTransactionalScopeBehavior_WhenCommandRuns_ThenHandlerSeesAmbientTransactionAsync()
    {
        // SQLite does not enlist in TransactionScope; assert the wrapper still flows an ambient transaction.
        using var host = new SqlitePipelineHost(TransactionBehaviorEnum.TransactionalScopeBehavior);

        var snapshot = await host.SendAsync(new TransactionProbeCommand());

        snapshot.HasAmbientTransaction.Should().BeTrue();
    }

    [Test]
    [Category(TestCategory.INTEGRATION)]
    public async Task GivenTransactionalBehavior_WhenHandlerFails_ThenRollsBackChangesAsync()
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
    public async Task GivenTransactionalBehavior_WhenHandlerFailsAfterAdd_ThenChangeTrackerHasNoPendingEntriesAsync()
    {
        using var host = new SqlitePipelineHost(TransactionBehaviorEnum.TransactionalBehavior);
        var name = Guid.NewGuid().ToString("N");

        await host.ExecuteAsync(async sp =>
        {
            var mediator = sp.GetRequiredService<IMediator>();
            var dbContext = sp.GetRequiredService<TestDbContext>();

            var act = () => mediator.Send(new FailingAddWithoutSaveCommand { Name = name });
            await act.Should().ThrowAsync<InvalidOperationException>();

            dbContext.ChangeTracker.Entries()
                .Should()
                .BeEmpty();

            return 0;
        });
    }

    [Test]
    [Category(TestCategory.INTEGRATION)]
    public async Task GivenTransactionalBehavior_WhenHandlerFailsAfterAdd_ThenKeepsPreCommandTrackedEntriesAsync()
    {
        using var host = new SqlitePipelineHost(TransactionBehaviorEnum.TransactionalBehavior);
        var committedName = Guid.NewGuid().ToString("N");
        var pendingName = Guid.NewGuid().ToString("N");
        var failedName = Guid.NewGuid().ToString("N");

        await host.ExecuteAsync(async sp =>
        {
            var mediator = sp.GetRequiredService<IMediator>();
            var dbContext = sp.GetRequiredService<TestDbContext>();

            await mediator.Send(new CreateTestEntityCommand { Name = committedName });

            dbContext.TestEntities.Add(new TestEntity
            {
                Name = pendingName,
                CreatedOn = DateTime.UtcNow
            });

            var act = () => mediator.Send(new FailingAddWithoutSaveCommand { Name = failedName });
            await act.Should().ThrowAsync<InvalidOperationException>();

            dbContext.ChangeTracker.Entries<TestEntity>()
                .Should()
                .HaveCount(2);
            dbContext.ChangeTracker.Entries<TestEntity>()
                .Should()
                .ContainSingle(entry => entry.State == EntityState.Unchanged && entry.Entity.Name == committedName);
            dbContext.ChangeTracker.Entries<TestEntity>()
                .Should()
                .ContainSingle(entry => entry.State == EntityState.Added && entry.Entity.Name == pendingName);
            dbContext.ChangeTracker.Entries<TestEntity>()
                .Should()
                .NotContain(entry => entry.Entity.Name == failedName);

            return 0;
        });
    }

    [Test]
    [Category(TestCategory.INTEGRATION)]
    public async Task GivenTransactionalBehavior_WhenCommandSucceeds_ThenChangeTrackerKeepsUnchangedEntriesAsync()
    {
        using var host = new SqlitePipelineHost(TransactionBehaviorEnum.TransactionalBehavior);
        var name = Guid.NewGuid().ToString("N");

        await host.ExecuteAsync(async sp =>
        {
            var mediator = sp.GetRequiredService<IMediator>();
            var dbContext = sp.GetRequiredService<TestDbContext>();

            await mediator.Send(new CreateTestEntityCommand { Name = name });

            dbContext.ChangeTracker.Entries<TestEntity>()
                .Should()
                .ContainSingle(entry => entry.State == EntityState.Unchanged && entry.Entity.Name == name);

            return 0;
        });
    }

    [Test]
    [Category(TestCategory.INTEGRATION)]
    public async Task GivenUnknownTransactionBehavior_WhenCommandSent_ThenThrowsAndDoesNotExecuteHandlerAsync()
    {
        using var host = new SqlitePipelineHost((TransactionBehaviorEnum)99);
        var name = Guid.NewGuid().ToString("N");

        var act = () => host.SendAsync(new CreateTestEntityCommand { Name = name });
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
            .WithParameterName("behavior");

        var entity = await host.ExecuteAsync(sp =>
            sp.GetRequiredService<TestDbContext>().TestEntities.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Name == name));

        entity.Should().BeNull();
    }

    [Test]
    [Category(TestCategory.INTEGRATION)]
    public async Task GivenNoBehavior_WhenHandlerFailsAfterSave_ThenChangesRemainAsync()
    {
        using var host = new SqlitePipelineHost(TransactionBehaviorEnum.NoBehavior);
        var name = Guid.NewGuid().ToString("N");

        var act = () => host.SendAsync(new FailingCreateTestEntityCommand { Name = name });
        await act.Should().ThrowAsync<InvalidOperationException>();

        var entity = await host.ExecuteAsync(sp =>
            sp.GetRequiredService<TestDbContext>().TestEntities.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Name == name));

        entity.Should().NotBeNull();
        entity!.Name.Should().Be(name);
    }

    [Test]
    [Category(TestCategory.INTEGRATION)]
    public async Task GivenNoBehavior_WhenHandlerAddsWithoutSave_ThenChangeTrackerKeepsAddedEntriesAsync()
    {
        using var host = new SqlitePipelineHost(TransactionBehaviorEnum.NoBehavior);
        var name = Guid.NewGuid().ToString("N");

        await host.ExecuteAsync(async sp =>
        {
            var mediator = sp.GetRequiredService<IMediator>();
            var dbContext = sp.GetRequiredService<TestDbContext>();

            await mediator.Send(new AddTestEntityWithoutSaveCommand { Name = name });

            dbContext.ChangeTracker.Entries<TestEntity>()
                .Should()
                .ContainSingle(entry => entry.State == EntityState.Added && entry.Entity.Name == name);

            return 0;
        });
    }

    [Test]
    [Category(TestCategory.INTEGRATION)]
    public async Task GivenNoBehavior_WhenHandlerFailsAfterAdd_ThenChangeTrackerKeepsAddedEntriesAsync()
    {
        using var host = new SqlitePipelineHost(TransactionBehaviorEnum.NoBehavior);
        var name = Guid.NewGuid().ToString("N");

        await host.ExecuteAsync(async sp =>
        {
            var mediator = sp.GetRequiredService<IMediator>();
            var dbContext = sp.GetRequiredService<TestDbContext>();

            var act = () => mediator.Send(new FailingAddWithoutSaveCommand { Name = name });
            await act.Should().ThrowAsync<InvalidOperationException>();

            dbContext.ChangeTracker.Entries<TestEntity>()
                .Should()
                .ContainSingle(entry => entry.State == EntityState.Added && entry.Entity.Name == name);

            return 0;
        });
    }

    [Test]
    [Category(TestCategory.INTEGRATION)]
    public async Task GivenTransactionalBehavior_WhenCommandRuns_ThenHandlerSeesEfTransactionAsync()
    {
        using var host = new SqlitePipelineHost(TransactionBehaviorEnum.TransactionalBehavior);

        var snapshot = await host.SendAsync(new TransactionProbeCommand());

        snapshot.HasEfTransaction.Should().BeTrue();
    }

    [Test]
    [Category(TestCategory.INTEGRATION)]
    public async Task GivenNoBehavior_WhenCommandRuns_ThenHandlerDoesNotSeeEfTransactionAsync()
    {
        using var host = new SqlitePipelineHost(TransactionBehaviorEnum.NoBehavior);

        var snapshot = await host.SendAsync(new TransactionProbeCommand());

        snapshot.HasEfTransaction.Should().BeFalse();
        snapshot.HasAmbientTransaction.Should().BeFalse();
    }

    [Test]
    [Category(TestCategory.INTEGRATION)]
    public async Task GivenTransactionalBehavior_WhenQueryRuns_ThenDoesNotOpenTransactionAsync()
    {
        using var host = new SqlitePipelineHost(TransactionBehaviorEnum.TransactionalBehavior);

        var snapshot = await host.SendAsync(new TransactionProbeQuery());

        snapshot.HasEfTransaction.Should().BeFalse();
        snapshot.HasAmbientTransaction.Should().BeFalse();
    }

    [Test]
    [Category(TestCategory.INTEGRATION)]
    public async Task GivenNoBehaviorAndSerializableGlobally_WhenExactTransactionRequestsReadCommitted_ThenBeginsWithReadCommittedAsync()
    {
        using var host = new SqlitePipelineHost(TransactionBehaviorEnum.NoBehavior, IsolationLevel.Serializable);

        var snapshot = await host.SendAsync(new ExactTransactionProbeCommand());

        snapshot.HasEfTransaction.Should().BeTrue();
        // SQLite reports Serializable on GetDbTransaction(); assert the isolation EF was asked to start.
        host.IsolationCapture.LastStartedIsolationLevel.Should().Be(System.Data.IsolationLevel.ReadCommitted);
    }

    [Test]
    [Category(TestCategory.INTEGRATION)]
    [TestCase(IsolationLevel.ReadUncommitted)]
    [TestCase(IsolationLevel.ReadCommitted)]
    [TestCase(IsolationLevel.RepeatableRead)]
    [TestCase(IsolationLevel.Serializable)]
    public async Task GivenTransactionalBehavior_WhenIsolationLevelConfigured_ThenCommandSucceedsAsync(IsolationLevel isolationLevel)
    {
        using var host = new SqlitePipelineHost(TransactionBehaviorEnum.TransactionalBehavior, isolationLevel);
        var name = Guid.NewGuid().ToString("N");

        await host.SendAsync(new CreateTestEntityCommand { Name = name });

        host.IsolationCapture.LastStartedIsolationLevel.Should().Be(isolationLevel.ToDataIsolation());

        var entity = await host.ExecuteAsync(sp =>
            sp.GetRequiredService<TestDbContext>().TestEntities.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Name == name));

        entity.Should().NotBeNull();
    }
}
