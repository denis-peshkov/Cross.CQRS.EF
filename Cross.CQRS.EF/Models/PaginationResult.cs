namespace Cross.CQRS.EF.Models;

public class PaginationResult<T>
{
    public IReadOnlyCollection<T> Data { get; set; } = Array.Empty<T>();

    public long Count { get; set; }
}
