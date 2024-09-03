namespace Microsoft.EntityFrameworkCore;

public static class DbContextOptionsBuilderExtensions
{
    public static DbContextOptionsBuilder EnableRemoveForeignKey(this DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.WithOption(e => e.WithRemoveForeignKey());

    public static DbContextOptionsBuilder IncludeXmlComments(this DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.WithOption(e => e.WithXmlCommentPath(Directory.GetFiles(AppContext.BaseDirectory, "*.xml")));

    public static DbContextOptionsBuilder IncludeXmlComments(this DbContextOptionsBuilder optionsBuilder, params string[] filePath)
        => optionsBuilder.WithOption(e => e.WithXmlCommentPath(filePath));

    public static DbContextOptionsBuilder UseSoftDelete(this DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.WithOption(e => e.WithSoftDelete());

    public static DbContextOptionsBuilder UseSoftDelete(this DbContextOptionsBuilder optionsBuilder, string name)
        => optionsBuilder.WithOption(e => e.WithSoftDelete(name, string.Empty));

    public static DbContextOptionsBuilder UseSoftDelete(this DbContextOptionsBuilder optionsBuilder, string name, string comment)
        => optionsBuilder.WithOption(e => e.WithSoftDelete(name, comment));

    private static DbContextOptionsBuilder WithOption(this DbContextOptionsBuilder optionsBuilder, Func<EntityFrameworkCoreOptionsExtension, EntityFrameworkCoreOptionsExtension> withFunc)
    {
        ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(
            withFunc(optionsBuilder.Options.FindExtension<EntityFrameworkCoreOptionsExtension>() ?? new EntityFrameworkCoreOptionsExtension()));

        return optionsBuilder;
    }
}