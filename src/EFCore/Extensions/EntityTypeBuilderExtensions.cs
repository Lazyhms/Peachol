namespace Microsoft.EntityFrameworkCore;

public static class EntityTypeBuilderExtensions
{
    public static EntityTypeBuilder HasStoredQueryFilter(this EntityTypeBuilder entityTypeBuilder, object storedKey, LambdaExpression filter)
        => entityTypeBuilder.HasAnnotation(EntityFrameworkCoreAnnotationNames.StoredQueryFilter, new SetQueryFilterCalls().SetFilter(storedKey, filter).Filters);

    public static EntityTypeBuilder HasStoredQueryFilter(this EntityTypeBuilder entityTypeBuilder, params LambdaExpression[] filters)
        => entityTypeBuilder.HasStoredQueryFilter((IEnumerable<LambdaExpression>)filters);

    public static EntityTypeBuilder HasStoredQueryFilter(this EntityTypeBuilder entityTypeBuilder, IEnumerable<LambdaExpression> filters)
    {
        var setQueryFilterCalls = new SetQueryFilterCalls();
        foreach (var item in filters)
        {
            setQueryFilterCalls.SetFilter(item);
        }
        return entityTypeBuilder.HasAnnotation(EntityFrameworkCoreAnnotationNames.StoredQueryFilter, setQueryFilterCalls.Filters);
    }

    public static EntityTypeBuilder HasStoredQueryFilter(this EntityTypeBuilder entityTypeBuilder, Func<SetQueryFilterCalls, SetQueryFilterCalls> queryFilterAction)
    => entityTypeBuilder.HasAnnotation(EntityFrameworkCoreAnnotationNames.StoredQueryFilter, queryFilterAction(new()).Filters);

    public static EntityTypeBuilder<TEntity> HasStoredQueryFilter<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder, object storedKey, Expression<Func<TEntity, bool>> filter) where TEntity : class
        => entityTypeBuilder.HasAnnotation(EntityFrameworkCoreAnnotationNames.StoredQueryFilter, new SetQueryFilterCalls<TEntity>().SetFilter(storedKey, filter).Filters);

    public static EntityTypeBuilder<TEntity> HasStoredQueryFilter<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder, params Expression<Func<TEntity, bool>>[] filters) where TEntity : class
        => entityTypeBuilder.HasStoredQueryFilter((IEnumerable<Expression<Func<TEntity, bool>>>)filters);

    public static EntityTypeBuilder<TEntity> HasStoredQueryFilter<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder, IEnumerable<Expression<Func<TEntity, bool>>> filters) where TEntity : class
    {
        var setQueryFilterCalls = new SetQueryFilterCalls<TEntity>();
        foreach (var item in filters)
        {
            setQueryFilterCalls.SetFilter(item);
        }
        return entityTypeBuilder.HasAnnotation(EntityFrameworkCoreAnnotationNames.StoredQueryFilter, setQueryFilterCalls.Filters);
    }

    public static EntityTypeBuilder<TEntity> HasStoredQueryFilter<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder, Func<SetQueryFilterCalls<TEntity>, SetQueryFilterCalls<TEntity>> queryFilterAction) where TEntity : class
        => entityTypeBuilder.HasAnnotation(EntityFrameworkCoreAnnotationNames.StoredQueryFilter, queryFilterAction(new()).Filters);
}
