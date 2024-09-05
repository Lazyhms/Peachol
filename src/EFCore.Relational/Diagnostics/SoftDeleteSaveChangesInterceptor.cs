﻿namespace Microsoft.EntityFrameworkCore.Diagnostics;

public class SoftDeleteSaveChangesInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
        {
            ProcessSoftRemoveSaveChanges(eventData.Context);
        }

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            ProcessSoftRemoveSaveChanges(eventData.Context);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void ProcessSoftRemoveSaveChanges(DbContext context)
    {
        foreach (var entityEntry in context.ChangeTracker.Entries())
        {
            if (EntityState.Deleted == entityEntry.State
                && entityEntry.Metadata.FindAnnotation(CoreAnnotationNames.SoftDelete) is IAnnotation annotation
                && annotation.Value is string propertyName
                && entityEntry.Property(propertyName) is PropertyEntry propertyEntry)
            {
                entityEntry.State = EntityState.Unchanged;

                propertyEntry.CurrentValue = true;
                propertyEntry.IsModified = true;
            }
        }
    }
}