namespace SampleWebApp.Modules.Some.Handlers;

public sealed record ExternalEvent : CommandEvent
{
    public ExternalEvent(Guid commandId, string message)
        : base(commandId)
    {
        Message = message;
    }

    public string Message { get; }
}
