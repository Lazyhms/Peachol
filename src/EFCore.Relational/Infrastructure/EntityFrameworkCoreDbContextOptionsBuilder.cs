﻿using System.ComponentModel;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;

namespace Microsoft.EntityFrameworkCore.Infrastructure;

public class EntityFrameworkCoreDbContextOptionsBuilder(DbContextOptionsBuilder optionsBuilder)
{
    private readonly Lazy<SoftDeleteSaveChangesInterceptor> _softDeleteSaveChangesInterceptor
        = new(() => new SoftDeleteSaveChangesInterceptor());

    public EntityFrameworkCoreDbContextOptionsBuilder EnableRemoveForeignKey()
        => WithOption(e => e.WithRemoveForeignKey());

    public EntityFrameworkCoreDbContextOptionsBuilder IncludeXmlComments()
        => WithOption(e => e.WithXmlCommentPath(Directory.GetFiles(AppContext.BaseDirectory, "*.xml")));

    public EntityFrameworkCoreDbContextOptionsBuilder IncludeXmlComments(params string[] filePath)
        => WithOption(e => e.WithXmlCommentPath(filePath));

    public EntityFrameworkCoreDbContextOptionsBuilder IncludeXmlComments(IEnumerable<string> filePath)
        => WithOption(e => e.WithXmlCommentPath(filePath));

    public EntityFrameworkCoreDbContextOptionsBuilder UseSoftDelete()
    {
        optionsBuilder.AddInterceptors(_softDeleteSaveChangesInterceptor.Value);
        return WithOption(e => e.WithSoftDelete());
    }

    public EntityFrameworkCoreDbContextOptionsBuilder UseSoftDelete(string name)
    {
        optionsBuilder.AddInterceptors(_softDeleteSaveChangesInterceptor.Value);
        return WithOption(e => e.WithSoftDelete(name, string.Empty));
    }

    public EntityFrameworkCoreDbContextOptionsBuilder UseSoftDelete(string name, string comment)
    {
        optionsBuilder.AddInterceptors(_softDeleteSaveChangesInterceptor.Value);
        return WithOption(e => e.WithSoftDelete(name, comment));
    }

    private EntityFrameworkCoreDbContextOptionsBuilder WithOption(Func<EntityFrameworkCoreDbContextOptionsExtension, EntityFrameworkCoreDbContextOptionsExtension> setAction)
    {
        var extension = optionsBuilder.Options.FindExtension<EntityFrameworkCoreDbContextOptionsExtension>() ?? new EntityFrameworkCoreDbContextOptionsExtension();
        ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(setAction(extension));

        return this;
    }

    #region Hidden System.Object members

    /// <summary>
    ///     Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override string? ToString()
        => base.ToString();

    /// <summary>
    ///     Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns><see langword="true" /> if the specified object is equal to the current object; otherwise, <see langword="false" />.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object? obj)
        => base.Equals(obj);

    /// <summary>
    ///     Serves as the default hash function.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode()
        => base.GetHashCode();

    #endregion
}