namespace Microsoft.EntityFrameworkCore;

internal static class EntityEntryExtensions
{
    public static EntityEntry<TEntity> SoftRemove<TEntity>(this EntityEntry<TEntity> entityEntry) where TEntity : class
    {
        if (!entityEntry.Metadata.ClrType.IsDefined<HardDeleteAttribute>())
        {
            if (entityEntry.Metadata.ClrType.GetCustomAttribute<SoftDeleteAttribute>() is not null and { Enable: true } softDeleteAttribute && !string.IsNullOrWhiteSpace(softDeleteAttribute.Name))
            {
                entityEntry.Property(softDeleteAttribute.Name).CurrentValue = true;
                entityEntry.Property(softDeleteAttribute.Name).IsModified = true;
            }
            else if (entityEntry.Context.GetService<IEntityFrameworkCoreSingletonOptions>() is IEntityFrameworkCoreSingletonOptions coreSingletonOptions
               && coreSingletonOptions.SoftDeleteOptions is not null and { Enabled: true } softDeleteOptions && !string.IsNullOrWhiteSpace(softDeleteOptions.Name))
            {
                entityEntry.Property(softDeleteOptions.Name).CurrentValue = true;
                entityEntry.Property(softDeleteOptions.Name).IsModified = true;
            }
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