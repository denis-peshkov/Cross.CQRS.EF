namespace Cross.CQRS.EF.Filters;

public interface IQueryableFilter<TQuery, TResult>
{
    IQueryable<TResult> Apply(IQueryable<TResult> source);
}
