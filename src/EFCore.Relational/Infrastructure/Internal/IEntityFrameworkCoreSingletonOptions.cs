namespace Microsoft.EntityFrameworkCore.Infrastructure.Internal;

public interface IEntityFrameworkCoreSingletonOptions : ISingletonOptions
{
    List<string> XmlCommentPath { get; set; }

    bool RemoveForeignKeyEnabled { get; set; }

    SoftDeleteOptions SoftDeleteOptions { get; set; }
}
