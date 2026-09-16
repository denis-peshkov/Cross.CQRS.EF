namespace SampleWebApp.Modules.Some.Handlers;

public record InternalEvent(Guid CommandId, string Message) : ICommandEvent;
