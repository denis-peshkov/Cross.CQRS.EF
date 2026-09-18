namespace SampleWebApp.Modules.Some.Handlers;

public sealed record InternalEvent : CommandEvent
{
    public InternalEvent(Guid commandId, string message)
        : base(commandId)
    {
        Message = message;
    }

    public string Message { get; }
}
