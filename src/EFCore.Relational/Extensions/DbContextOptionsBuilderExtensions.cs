namespace Microsoft.EntityFrameworkCore;

public static class DbContextOptionsBuilderExtensions
{
    private static readonly Lazy<SoftDeleteSaveChangesInterceptor> _softDeleteSaveChangesInterceptor 
        = new(() => new SoftDeleteSaveChangesInterceptor());

    public static DbContextOptionsBuilder EnableRemoveForeignKey(this DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.WithOption(e => e.WithRemoveForeignKey());

    public static DbContextOptionsBuilder IncludeXmlComments(this DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.WithOption(e => e.WithXmlCommentPath(Directory.GetFiles(AppContext.BaseDirectory, "*.xml")));

    public static DbContextOptionsBuilder IncludeXmlComments(this DbContextOptionsBuilder optionsBuilder, params string[] filePath)
        => optionsBuilder.WithOption(e => e.WithXmlCommentPath(filePath));

    public static DbContextOptionsBuilder UseSoftDelete(this DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.WithOption(e => e.WithSoftDelete()).AddInterceptors(_softDeleteSaveChangesInterceptor.Value);

    public static DbContextOptionsBuilder UseSoftDelete(this DbContextOptionsBuilder optionsBuilder, string name)
        => optionsBuilder.WithOption(e => e.WithSoftDelete(name, string.Empty)).AddInterceptors(_softDeleteSaveChangesInterceptor.Value);

    public static DbContextOptionsBuilder UseSoftDelete(this DbContextOptionsBuilder optionsBuilder, string name, string comment)
        => optionsBuilder.WithOption(e => e.WithSoftDelete(name, comment)).AddInterceptors(_softDeleteSaveChangesInterceptor.Value);

    private static DbContextOptionsBuilder WithOption(this DbContextOptionsBuilder optionsBuilder, Func<EntityFrameworkCoreOptionsExtension, EntityFrameworkCoreOptionsExtension> withFunc)
    {
        ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(
            withFunc(optionsBuilder.Options.FindExtension<EntityFrameworkCoreOptionsExtension>() ?? new EntityFrameworkCoreOptionsExtension()));

        return optionsBuilder;
    }
}