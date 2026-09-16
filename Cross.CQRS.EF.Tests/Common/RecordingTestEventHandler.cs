namespace Cross.CQRS.EF.Tests.Common;

internal sealed class RecordingTestEventHandler : INotificationHandler<TestEvent>
{
    private readonly List<TestEvent> _published = new();

    public IReadOnlyList<TestEvent> Published => _published;

    public Task Handle(TestEvent notification, CancellationToken cancellationToken)
    {
        _published.Add(notification);
        return Task.CompletedTask;
    }
}
