namespace SampleWebApp.Modules.Some.Handlers;

public record ExternalEvent(Guid CommandId, string Message) : ICommandEvent;
