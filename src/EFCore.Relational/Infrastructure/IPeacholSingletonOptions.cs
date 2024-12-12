namespace Microsoft.EntityFrameworkCore.Infrastructure;

public interface IPeacholSingletonOptions : ISingletonOptions
{
    List<string> XmlCommentPath { get; set; }

    bool RemoveForeignKeyEnabled { get; set; }

    SoftDeleteOptions SoftDeleteOptions { get; set; }
}
