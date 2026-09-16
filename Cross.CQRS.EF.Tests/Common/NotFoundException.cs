namespace Cross.CQRS.EF.Tests.Common;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}