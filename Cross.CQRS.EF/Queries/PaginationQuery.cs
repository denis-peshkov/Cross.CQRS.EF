namespace Cross.CQRS.EF.Queries;

/// <summary>
/// Extension of <see cref="Query{TResult}"/> for paginated queries.
/// </summary>
public abstract class PaginationQuery<TFilter, TResult> : Query<PaginationResult<TResult>>
{
    public PaginationRequest<TFilter> Request { get; }

    protected PaginationQuery(PaginationRequest<TFilter> request)
    {
        Request = request;
    }
}
