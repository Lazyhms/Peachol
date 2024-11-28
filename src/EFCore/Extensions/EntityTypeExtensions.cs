namespace Microsoft.EntityFrameworkCore;

public static class EntityTypeExtensions
{
    public static string? GetSoftDelete(this IEntityType entityType)
    {
        if (entityType.FindAnnotation(EntityFrameworkCoreAnnotationNames.SoftDelete) is IAnnotation annotation and not null
            && annotation.Value is string column and not null && !string.IsNullOrWhiteSpace(column))
        {
            return column;
        }
        return null;
    }

    public static IConventionAnnotation? SetOrRemoveSoftDelete(this IConventionEntityType conventionEntityType, string column)
        => conventionEntityType.SetOrRemoveAnnotation(EntityFrameworkCoreAnnotationNames.SoftDelete, column, true);
}
