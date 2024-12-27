using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Microsoft.EntityFrameworkCore;

public static class EntityFrameworkCoreQueryableExtensions
{
    public static IOrderedQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> source, string propertyOrFieldName) where TEntity : class
    {
        var parameter = Expression.Parameter(typeof(TEntity));
        var member = Expression.PropertyOrField(parameter, propertyOrFieldName);

        return (IOrderedQueryable<TEntity>)
            (source.Provider is EntityQueryProvider
                ? source.Provider.CreateQuery<TEntity>(
                    Expression.Call(
                        null,
                        QueryableMethods.OrderBy.MakeGenericMethod(parameter.Type, member.Type),
                        [source.Expression, Expression.Lambda(member, parameter)]))
                 : source);
    }

    public static IOrderedQueryable<TEntity> OrderByDescending<TEntity>(this IQueryable<TEntity> source, string propertyOrFieldName) where TEntity : class
    {
        var parameter = Expression.Parameter(typeof(TEntity));
        var member = Expression.PropertyOrField(parameter, propertyOrFieldName);

        return (IOrderedQueryable<TEntity>)
            (source.Provider is EntityQueryProvider
                ? source.Provider.CreateQuery<TEntity>(
                    Expression.Call(
                        null,
                        QueryableMethods.OrderByDescending.MakeGenericMethod(parameter.Type, member.Type),
                        [source.Expression, Expression.Lambda(member, parameter)]))
                 : source);
    }

    public static IOrderedQueryable<TEntity> ThenBy<TEntity>(this IOrderedQueryable<TEntity> source, string propertyOrFieldName) where TEntity : class
    {
        var parameter = Expression.Parameter(typeof(TEntity));
        var member = Expression.PropertyOrField(parameter, propertyOrFieldName);

        return (IOrderedQueryable<TEntity>)
            (source.Provider is EntityQueryProvider
                ? source.Provider.CreateQuery<TEntity>(
                    Expression.Call(null,
                    QueryableMethods.ThenBy.MakeGenericMethod(parameter.Type, member.Type),
                    [source.Expression, Expression.Lambda(member, parameter)]))
                : source);
    }

    public static IOrderedQueryable<TEntity> ThenByDescending<TEntity>(this IOrderedQueryable<TEntity> source, string propertyOrFieldName) where TEntity : class
    {
        var parameter = Expression.Parameter(typeof(TEntity));
        var member = Expression.PropertyOrField(parameter, propertyOrFieldName);

        return (IOrderedQueryable<TEntity>)
            (source.Provider is EntityQueryProvider
                ? source.Provider.CreateQuery<TEntity>(
                    Expression.Call(null,
                    QueryableMethods.ThenByDescending.MakeGenericMethod(parameter.Type, member.Type),
                    [source.Expression, Expression.Lambda(member, parameter)]))
                : source);
    }

    public static Pagination<TEntity> Pagination<TEntity>(this IQueryable<TEntity> source, int pageIndex, int pageSize)
    {
        var totalCount = source.Count();
        return 0 == totalCount
            ? new Pagination<TEntity>()
            : new Pagination<TEntity> { TotalCount = totalCount, Data = [.. source.Skip(pageSize * (pageIndex - 1)).Take(pageSize)] };
    }

    public static async Task<Pagination<TEntity>> PaginationAsync<TEntity>(this IQueryable<TEntity> source, int pageIndex, int pageSize)
    {
        var totalCount = await source.CountAsync();
        return 0 == totalCount
            ? new Pagination<TEntity>()
            : new Pagination<TEntity> { TotalCount = totalCount, Data = await source.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToListAsync() };
    }

    public static IQueryable<TResult> LeftJoin<TOuter, TInner, TKey, TResult>(
        this IQueryable<TOuter> outer,
        IEnumerable<TInner> inner,
        Expression<Func<TOuter, TKey>> outerKeySelector,
        Expression<Func<TInner, TKey>> innerKeySelector,
        Expression<Func<JoinedClass<TOuter, TInner>, TResult>> resultSelector)
        => outer.GroupJoin(inner, outerKeySelector, innerKeySelector, (outer, inner) => new { Outer = outer, Inner = inner })
            .SelectMany(sm => sm.Inner.DefaultIfEmpty(), (o, i) => new JoinedClass<TOuter, TInner> { Outer = o.Outer, Inner = i }).Select(resultSelector);
}