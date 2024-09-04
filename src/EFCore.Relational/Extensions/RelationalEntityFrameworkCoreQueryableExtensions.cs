namespace Microsoft.EntityFrameworkCore;

public static class RelationalEntityFrameworkCoreQueryableExtensions
{
    public static int ExecuteSoftRemove<TSource>(this IQueryable<TSource> source) where TSource : class
    {
        if (source is DbSet<TSource> dbSet && dbSet.EntityType.GetProperties().SingleOrDefault(
                sd => sd.FindAnnotation(CoreAnnotationNames.SoftDelete) is not null) is IProperty softDeleteProperty)
        {
            return source.ExecuteUpdate(setPropertyCalls =>
                setPropertyCalls.SetProperty(
                    property =>
                        EF.Property<bool>(property, softDeleteProperty.Name),
                        value => true));
        }

        throw new InvalidOperationException("Soft Delete not enabled");
    }

    public static Task<int> ExecuteSoftRemoveAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default) where TSource : class
    {
        if (source is DbSet<TSource> dbSet && dbSet.EntityType.GetProperties().SingleOrDefault(
                sd => sd.FindAnnotation(CoreAnnotationNames.SoftDelete) is not null) is IProperty softDeleteProperty)
        {
            return source.ExecuteUpdateAsync(setPropertyCalls =>
                setPropertyCalls.SetProperty(
                    property =>
                        EF.Property<bool>(property, softDeleteProperty.Name),
                        value => true),
                cancellationToken);
        }

        throw new InvalidOperationException("Soft Delete not enabled");
    }
}
