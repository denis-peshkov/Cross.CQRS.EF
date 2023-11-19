namespace Cross.CQRS.EF.Queries;

public abstract class PaginationQueryHandler<TQuery, TFilter, TResult> : QueryHandler<TQuery, PaginationResult<TResult>>
    where TQuery : PaginationQuery<TFilter, TResult>
{
    private readonly IEnumerable<IQueryableFilter<TQuery, TResult>> _filters;

    protected PaginationQueryHandler(IEnumerable<IQueryableFilter<TQuery, TResult>> filters)
    {
        _filters = filters;
    }

    protected override async Task<PaginationResult<TResult>> HandleAsync(TQuery query, CancellationToken cancellationToken)
    {
        var queryable = _filters.Aggregate(await GetQueryableAsync(query.Request.Filter, cancellationToken),
            (current, filter) => filter.Apply(current));

        var total = await queryable.CountAsync(cancellationToken);
        var pageQueryable = queryable;

        if (query.Request.Sort is { Count: > 0 })
        {
            pageQueryable = pageQueryable.Sort(query.Request.Sort);
        }

        if (query.Request.Skip > 0)
        {
            pageQueryable = pageQueryable.Skip(query.Request.Skip);
        }

        if (query.Request.Take > 0)
        {
            pageQueryable = pageQueryable.Take(query.Request.Take);
        }

        var page = await MaterializeAsync(pageQueryable, cancellationToken);
        return new PaginationResult<TResult>
        {
            Count = total,
            Data = page
        };
    }

    protected abstract Task<IQueryable<TResult>> GetQueryableAsync(TFilter filter, CancellationToken cancellation);

    protected virtual async Task<IReadOnlyCollection<TResult>> MaterializeAsync(IQueryable<TResult> pageQuery, CancellationToken cancellation = default)
        => await pageQuery.ToArrayAsync(cancellationToken: cancellation);
}
