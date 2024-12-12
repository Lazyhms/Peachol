namespace Microsoft.EntityFrameworkCore;

public static class DbContextOptionsBuilderExtensions
{
    public static DbContextOptionsBuilder UsePeachol(
        this DbContextOptionsBuilder optionsBuilder,
        Action<PeacholDbContextOptionsBuilder>? dbContextOptionsAction = null)
    {
        var extension = GetOrCreateExtension(optionsBuilder);
        ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

        dbContextOptionsAction?.Invoke(new PeacholDbContextOptionsBuilder(optionsBuilder));

        return optionsBuilder;
    }

    private static PeacholDbContextOptionsExtension GetOrCreateExtension(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.Options.FindExtension<PeacholDbContextOptionsExtension>()
            ?? new PeacholDbContextOptionsExtension();
}