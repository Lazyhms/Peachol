using System.ComponentModel;

namespace Microsoft.EntityFrameworkCore.Infrastructure;

public class PeacholDbContextOptionsBuilder(DbContextOptionsBuilder optionsBuilder)
{
    private readonly Lazy<SoftDeleteSaveChangesInterceptor> _softDeleteSaveChangesInterceptor
        = new(() => new SoftDeleteSaveChangesInterceptor());

    public PeacholDbContextOptionsBuilder EnableRemoveForeignKey()
        => WithOption(e => e.WithRemoveForeignKey());

    public PeacholDbContextOptionsBuilder IncludeXmlComments()
        => WithOption(e => e.WithXmlCommentPath(Directory.GetFiles(AppContext.BaseDirectory, "*.xml")));

    public PeacholDbContextOptionsBuilder IncludeXmlComments(params string[] filePath)
        => WithOption(e => e.WithXmlCommentPath(filePath));

    public PeacholDbContextOptionsBuilder IncludeXmlComments(IEnumerable<string> filePath)
        => WithOption(e => e.WithXmlCommentPath(filePath));

    public PeacholDbContextOptionsBuilder UseSoftDelete()
    {
        optionsBuilder.AddInterceptors(_softDeleteSaveChangesInterceptor.Value);
        return WithOption(e => e.WithSoftDelete());
    }

    public PeacholDbContextOptionsBuilder UseSoftDelete(string name)
    {
        optionsBuilder.AddInterceptors(_softDeleteSaveChangesInterceptor.Value);
        return WithOption(e => e.WithSoftDelete(name, string.Empty));
    }

    public PeacholDbContextOptionsBuilder UseSoftDelete(string name, string comment)
    {
        optionsBuilder.AddInterceptors(_softDeleteSaveChangesInterceptor.Value);
        return WithOption(e => e.WithSoftDelete(name, comment));
    }

    private PeacholDbContextOptionsBuilder WithOption(Func<PeacholDbContextOptionsExtension, PeacholDbContextOptionsExtension> setAction)
    {
        var extension = optionsBuilder.Options.FindExtension<PeacholDbContextOptionsExtension>() ?? new PeacholDbContextOptionsExtension();
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
