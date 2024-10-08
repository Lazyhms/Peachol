﻿namespace Microsoft.EntityFrameworkCore;

public static class RelationalEntityFrameworkCoreQueryableExtensions
{
    /// <summary>
    /// Soft deletes database rows for the entity instances which match the LINQ query from the database.
    /// </summary>
    /// <param name="source">The source query.</param>
    /// <returns>The total number of rows deleted in the database.</returns>
    /// <exception cref="InvalidOperationException">
    /// This is usually because soft deletion is not enabled
    /// </exception>
    public static int ExecuteSoftDelete<TSource>(this IQueryable<TSource> source) where TSource : class
    {
        if (source is DbSet<TSource> dbSet && dbSet.EntityType.FindAnnotation(CoreAnnotationNames.SoftDelete) is IAnnotation annotation
            && annotation.Value is string propertyName && dbSet.EntityType.FindProperty(propertyName) is IProperty softDeleteProperty)
        {
            return source.ExecuteUpdate(setPropertyCalls =>
                setPropertyCalls.SetProperty(
                    property =>
                        EF.Property<bool>(property, softDeleteProperty.Name),
                    value => true));
        }

        throw new InvalidOperationException("Soft delete not enabled");
    }

    /// <summary>
    /// Asynchronously soft deletes database rows for the entity instances which match the LINQ query from the database.
    /// </summary>
    /// <param name="source">The source query.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>The total number of rows deleted in the database.</returns>
    /// <exception cref="InvalidOperationException">
    /// This is usually because soft deletion is not enabled
    /// </exception>
    public static Task<int> ExecuteSoftDeleteAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default) where TSource : class
    {
        if (source is DbSet<TSource> dbSet && dbSet.EntityType.FindAnnotation(CoreAnnotationNames.SoftDelete) is IAnnotation annotation
            && annotation.Value is string propertyName && dbSet.EntityType.FindProperty(propertyName) is IProperty softDeleteProperty)
        {
            return source.ExecuteUpdateAsync(setPropertyCalls =>
                setPropertyCalls.SetProperty(
                    property =>
                        EF.Property<bool>(property, softDeleteProperty.Name),
                    value => true),
                cancellationToken);
        }

        throw new InvalidOperationException("Soft delete not enabled");
    }

    /// <summary>
    /// Deletes or soft deletes database rows for the entity instances which match the LINQ query from the database.
    /// </summary>
    /// <param name="source">The source query.</param>
    /// <returns>The total number of rows deleted in the database.</returns>
    public static int ExecuteDeleteOrSoftDelete<TSource>(this IQueryable<TSource> source) where TSource : class
    {
        if (source is DbSet<TSource> dbSet && dbSet.EntityType.FindAnnotation(CoreAnnotationNames.SoftDelete) is IAnnotation annotation
            && annotation.Value is string propertyName && dbSet.EntityType.FindProperty(propertyName) is IProperty softDeleteProperty)
        {
            return source.ExecuteUpdate(setPropertyCalls =>
                setPropertyCalls.SetProperty(
                    property =>
                        EF.Property<bool>(property, softDeleteProperty.Name),
                    value => true));
        }
        return source.ExecuteDelete();
    }

    /// <summary>
    /// Asynchronously deletes or soft deletes database rows for the entity instances which match the LINQ query from the database.
    /// </summary>
    /// <param name="source">The source query.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>The total number of rows deleted in the database.</returns>
    public static Task<int> ExecuteDeleteOrSoftDeleteAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default) where TSource : class
    {
        if (source is DbSet<TSource> dbSet && dbSet.EntityType.FindAnnotation(CoreAnnotationNames.SoftDelete) is IAnnotation annotation
            && annotation.Value is string propertyName && dbSet.EntityType.FindProperty(propertyName) is IProperty softDeleteProperty)
        {
            return source.ExecuteUpdateAsync(setPropertyCalls =>
                setPropertyCalls.SetProperty(
                    property =>
                        EF.Property<bool>(property, softDeleteProperty.Name),
                    value => true),
                cancellationToken);
        }
        return source.ExecuteDeleteAsync(cancellationToken);
    }
}
