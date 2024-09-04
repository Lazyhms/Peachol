namespace Microsoft.EntityFrameworkCore;

internal static class EntityEntryExtensions
{
    public static EntityEntry<TEntity> SoftRemove<TEntity>(this EntityEntry<TEntity> entityEntry) where TEntity : class
    {
        if (entityEntry.Properties.SingleOrDefault(sd =>
                sd.Metadata.FindAnnotation(CoreAnnotationNames.SoftDelete) is not null)
            is PropertyEntry softDeletePropertyEntry)
        {
            softDeletePropertyEntry.CurrentValue = true;
            softDeletePropertyEntry.IsModified = true;

            return entityEntry;
        }

        throw new InvalidOperationException("Soft Delete not enabled");
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