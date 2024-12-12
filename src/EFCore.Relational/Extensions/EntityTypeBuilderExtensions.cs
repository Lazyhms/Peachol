namespace Microsoft.EntityFrameworkCore;

public static class EntityTypeBuilderExtensions
{
    public static EntityTypeBuilder<TEntity> HasQueryFilter<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder, Dictionary<object, LambdaExpression> filter) where TEntity : class
    {
        return entityTypeBuilder.HasAnnotation("QueryFilterNamed", filter);
    }
}