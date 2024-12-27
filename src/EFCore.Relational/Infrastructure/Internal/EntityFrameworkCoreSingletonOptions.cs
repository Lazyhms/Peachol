namespace Microsoft.EntityFrameworkCore.Infrastructure.Internal;

public class EntityFrameworkCoreSingletonOptions : IEntityFrameworkCoreSingletonOptions
{
    public bool RemoveForeignKeyEnabled { get; set; } = false;

    public List<string> XmlCommentPath { get; set; } = [];

    public SoftDeleteOptions SoftDeleteOptions { get; set; } = new SoftDeleteOptions();

    public void Initialize(IDbContextOptions options)
    {
        var metioCoreOptionsExtension = options.FindExtension<EntityFrameworkCoreDbContextOptionsExtension>();

        if (null != metioCoreOptionsExtension)
        {
            XmlCommentPath = metioCoreOptionsExtension.XmlCommentPath;
            SoftDeleteOptions = metioCoreOptionsExtension.SoftDeleteOptions;
            RemoveForeignKeyEnabled = metioCoreOptionsExtension.RemoveForeignKeyEnabled;
        }
    }

    public void Validate(IDbContextOptions options)
    {
        var metioCoreOptionsExtension = options.FindExtension<EntityFrameworkCoreDbContextOptionsExtension>();

        if (null != metioCoreOptionsExtension
            && XmlCommentPath.Count != metioCoreOptionsExtension.XmlCommentPath.Count)
        {
            throw new InvalidOperationException(
                CoreStrings.SingletonOptionChanged(
                    nameof(EntityFrameworkCoreDbContextOptionsBuilder.IncludeXmlComments),
                    nameof(DbContextOptionsBuilder.UseInternalServiceProvider)));
        }

        if (null != metioCoreOptionsExtension
            && RemoveForeignKeyEnabled != metioCoreOptionsExtension.RemoveForeignKeyEnabled)
        {
            throw new InvalidOperationException(
                CoreStrings.SingletonOptionChanged(
                    nameof(EntityFrameworkCoreDbContextOptionsBuilder.EnableRemoveForeignKey),
                    nameof(DbContextOptionsBuilder.UseInternalServiceProvider)));
        }

        if (null != metioCoreOptionsExtension
            && SoftDeleteOptions != metioCoreOptionsExtension.SoftDeleteOptions)
        {
            throw new InvalidOperationException(
                CoreStrings.SingletonOptionChanged(
                    nameof(EntityFrameworkCoreDbContextOptionsBuilder.UseSoftDelete),
                    nameof(DbContextOptionsBuilder.UseInternalServiceProvider)));
        }
    }
}