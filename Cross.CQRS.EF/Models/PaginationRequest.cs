namespace Cross.CQRS.EF.Models;

public class PaginationRequest<TFilter>
{
    public TFilter Filter { get; set; }

    public int Take { get; set; }

    public int Skip { get; set; }

    public IReadOnlyCollection<SortParameter> Sort { get; set; } = Array.Empty<SortParameter>();
}
