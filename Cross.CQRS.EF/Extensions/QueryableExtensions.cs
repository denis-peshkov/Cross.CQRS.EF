namespace Cross.CQRS.EF.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> Sort<T>(this IQueryable<T> source, IEnumerable<SortParameter> parameters)
    {
        foreach (var sortParameter in parameters.Select((p, i) => new { p.Property, Direction = p.Direction, Index = i }))
        {
            var propertyPath = sortParameter.Property.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .ToArray();

            var parameter = Expression.Parameter(typeof(T), "p");
            var propertyAccess = GetPropertyAccessExpression<T>(propertyPath, parameter);
            var sortExpression = Expression.Lambda(propertyAccess, parameter);

            string sortMethodName;

            if (sortParameter.Index == 0)
            {
                sortMethodName = (sortParameter.Direction == SortDirectionEnum.Ascending)
                    ? nameof(Queryable.OrderBy)
                    : nameof(Queryable.OrderByDescending);
            }
            else
            {
                sortMethodName = (sortParameter.Direction == SortDirectionEnum.Ascending)
                    ? nameof(Queryable.ThenBy)
                    : nameof(Queryable.ThenByDescending);
            }

            var typeParameters = new[] { source.ElementType, sortExpression.Body.Type };
            var methodCall = Expression.Call(typeof(Queryable), sortMethodName, typeParameters,
                source.Expression, sortExpression);

            source = source.Provider.CreateQuery<T>(methodCall);
        }

        return source;
    }


    private static MemberExpression GetPropertyAccessExpression<T>(IEnumerable<string> propertyPath, Expression parameter)
    {
        var leftOperandType = typeof(T);
        var propertyAccess = (MemberExpression)null;

        foreach (var part in propertyPath)
        {
            var matchedProperty = leftOperandType.GetRuntimeProperties()
                .FirstOrDefault(p => p.Name.Equals(part, StringComparison.OrdinalIgnoreCase));

            if (matchedProperty == null)
            {
                throw new InvalidOperationException($"Unable to sort query, type '{leftOperandType.Name}' misses property '{part}'");
            }

            propertyAccess = Expression.Property(parameter, matchedProperty);
            parameter = propertyAccess;
            leftOperandType = matchedProperty.PropertyType;
        }

        return propertyAccess;
    }
}
