namespace Cross.CQRS.EF.Tests.Modules;

public sealed class ExactTransactionReadUncommittedProbeCommand : Command<TransactionSnapshot>
{
}

[ExactTransaction(TransactionBehaviorEnum.TransactionalBehavior, IsolationLevel.ReadUncommitted)]
public sealed class ExactTransactionReadUncommittedProbeCommandHandler
    : CommandHandler<ExactTransactionReadUncommittedProbeCommand, TransactionSnapshot>
{
    private readonly TestDbContext _dbContext;

    public ExactTransactionReadUncommittedProbeCommandHandler(
        ICommandEventQueueWriter commandEvents,
        ILogger<ExactTransactionReadUncommittedProbeCommandHandler> logger,
        TestDbContext dbContext)
        : base(commandEvents, logger)
    {
        _dbContext = dbContext;
    }

    protected override Task<TransactionSnapshot> HandleAsync(
        ExactTransactionReadUncommittedProbeCommand command,
        CancellationToken cancellationToken)
        => Task.FromResult(TransactionSnapshotFactory.Capture(_dbContext));
}

public sealed class ExactTransactionRepeatableReadProbeCommand : Command<TransactionSnapshot>
{
}

[ExactTransaction(TransactionBehaviorEnum.TransactionalBehavior, IsolationLevel.RepeatableRead)]
public sealed class ExactTransactionRepeatableReadProbeCommandHandler
    : CommandHandler<ExactTransactionRepeatableReadProbeCommand, TransactionSnapshot>
{
    private readonly TestDbContext _dbContext;

    public ExactTransactionRepeatableReadProbeCommandHandler(
        ICommandEventQueueWriter commandEvents,
        ILogger<ExactTransactionRepeatableReadProbeCommandHandler> logger,
        TestDbContext dbContext)
        : base(commandEvents, logger)
    {
        _dbContext = dbContext;
    }

    protected override Task<TransactionSnapshot> HandleAsync(
        ExactTransactionRepeatableReadProbeCommand command,
        CancellationToken cancellationToken)
        => Task.FromResult(TransactionSnapshotFactory.Capture(_dbContext));
}

public sealed class ExactTransactionSerializableProbeCommand : Command<TransactionSnapshot>
{
}

[ExactTransaction(TransactionBehaviorEnum.TransactionalBehavior, IsolationLevel.Serializable)]
public sealed class ExactTransactionSerializableProbeCommandHandler
    : CommandHandler<ExactTransactionSerializableProbeCommand, TransactionSnapshot>
{
    private readonly TestDbContext _dbContext;

    public ExactTransactionSerializableProbeCommandHandler(
        ICommandEventQueueWriter commandEvents,
        ILogger<ExactTransactionSerializableProbeCommandHandler> logger,
        TestDbContext dbContext)
        : base(commandEvents, logger)
    {
        _dbContext = dbContext;
    }

    protected override Task<TransactionSnapshot> HandleAsync(
        ExactTransactionSerializableProbeCommand command,
        CancellationToken cancellationToken)
        => Task.FromResult(TransactionSnapshotFactory.Capture(_dbContext));
}
