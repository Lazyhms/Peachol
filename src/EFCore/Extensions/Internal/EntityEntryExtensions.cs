namespace Microsoft.EntityFrameworkCore;

internal static class EntityEntryExtensions
{
    public static EntityEntry<TEntity> SoftRemove<TEntity>(this EntityEntry<TEntity> entityEntry) where TEntity : class
    {
        if (entityEntry.Metadata.FindAnnotation(CoreAnnotationNames.SoftDelete) is IAnnotation annotation
            && annotation.Value is string propertyName && entityEntry.Property(propertyName) is PropertyEntry propertyEntry)
        {
            propertyEntry.IsModified = true;
            propertyEntry.CurrentValue = true;

            return entityEntry;
        }

        throw new InvalidOperationException("Soft delete not enabled");
    }

    public static EntityEntry<TEntity> UpdateIngoreProperty<TEntity, TProperty>(this EntityEntry<TEntity> entityEntry, Expression<Func<TEntity, TProperty>> ingoreKeySelector) where TEntity : class
    {
        foreach (var item in ingoreKeySelector.GetMemberAccessList())
        {
            entityEntry.Property(item.Name).IsModified = false;
        }
        return entityEntry;
    }
}