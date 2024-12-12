using Microsoft.EntityFrameworkCore.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Globalization;
using System.Text;

namespace Microsoft.EntityFrameworkCore.Infrastructure;

public class PeacholDbContextOptionsExtension : IDbContextOptionsExtension
{
    private bool _removeForeignKeyEnabled;
    private SoftDeleteOptions _softDeleteOptions;
    private DbContextOptionsExtensionInfo? _info;
    private List<string> _xPathDocumentPath;

    public PeacholDbContextOptionsExtension()
    {
        _xPathDocumentPath = [];
        _softDeleteOptions = new SoftDeleteOptions();
    }

    protected PeacholDbContextOptionsExtension(PeacholDbContextOptionsExtension copyFrom)
    {
        _softDeleteOptions = copyFrom._softDeleteOptions;
        _xPathDocumentPath = copyFrom._xPathDocumentPath;
    }

    public DbContextOptionsExtensionInfo Info
        => _info ??= new ExtensionInfo(this);

    protected virtual PeacholDbContextOptionsExtension Clone()
        => new(this);

    public virtual PeacholDbContextOptionsExtension WithRemoveForeignKey()
    {
        var clone = Clone();

        clone._removeForeignKeyEnabled = true;

        return clone;
    }

    public virtual PeacholDbContextOptionsExtension WithSoftDelete()
    {
        var clone = Clone();

        clone._softDeleteOptions = new SoftDeleteOptions { Enabled = true };

        return clone;
    }

    public virtual PeacholDbContextOptionsExtension WithSoftDelete(string name)
    {
        var clone = Clone();

        clone._softDeleteOptions = new SoftDeleteOptions(name, string.Empty) { Enabled = true };

        return clone;
    }

    public virtual PeacholDbContextOptionsExtension WithSoftDelete(string name, string comment)
    {
        var clone = Clone();

        clone._softDeleteOptions = new SoftDeleteOptions(name, comment) { Enabled = true };

        return clone;
    }

    public virtual PeacholDbContextOptionsExtension WithXmlCommentPath(params string[] filePath)
    {
        var clone = Clone();

        clone._xPathDocumentPath = [.. clone._xPathDocumentPath, .. filePath];

        return clone;
    }

    public virtual bool RemoveForeignKeyEnabled
        => _removeForeignKeyEnabled;

    public virtual SoftDeleteOptions SoftDeleteOptions
        => _softDeleteOptions;

    public virtual List<string> XmlCommentPath
        => _xPathDocumentPath;

    public virtual void ApplyServices(IServiceCollection services) 
        => services.AddEntityFrameworkCoreServices();

    public virtual void Validate(IDbContextOptions options)
    {
        var metioCoreOptionsExtension = options.FindExtension<PeacholDbContextOptionsExtension>();

        if (null != metioCoreOptionsExtension
            && _xPathDocumentPath.Count != metioCoreOptionsExtension._xPathDocumentPath.Count)
        {
            throw new InvalidOperationException(
                CoreStrings.SingletonOptionChanged(
                    nameof(PeacholDbContextOptionsBuilder.IncludeXmlComments),
                    nameof(DbContextOptionsBuilder.UseInternalServiceProvider)));
        }

        if (null != metioCoreOptionsExtension
            && _removeForeignKeyEnabled != metioCoreOptionsExtension._removeForeignKeyEnabled)
        {
            throw new InvalidOperationException(
                CoreStrings.SingletonOptionChanged(
                    nameof(PeacholDbContextOptionsBuilder.EnableRemoveForeignKey),
                    nameof(DbContextOptionsBuilder.UseInternalServiceProvider)));
        }

        if (null != metioCoreOptionsExtension
            && _softDeleteOptions != metioCoreOptionsExtension._softDeleteOptions)
        {
            throw new InvalidOperationException(
                CoreStrings.SingletonOptionChanged(
                    nameof(PeacholDbContextOptionsBuilder.UseSoftDelete),
                    nameof(DbContextOptionsBuilder.UseInternalServiceProvider)));
        }
    }

    protected sealed class ExtensionInfo(PeacholDbContextOptionsExtension extension) : DbContextOptionsExtensionInfo(extension)
    {
        private int? _serviceProviderHash;
        private string? _logFragment;

        private new PeacholDbContextOptionsExtension Extension
            => (PeacholDbContextOptionsExtension)base.Extension;

        public override bool IsDatabaseProvider
            => false;

        public override string LogFragment
        {
            get
            {
                if (_logFragment == null)
                {
                    var builder = new StringBuilder();

                    if (Extension._removeForeignKeyEnabled)
                    {
                        builder.Append("NoneForeignKeyEnabled ");
                    }
                    if (Extension._softDeleteOptions.Enabled)
                    {
                        builder.Append(Extension._softDeleteOptions).Append(' ');
                    }

                    _logFragment = builder.ToString();
                }

                return _logFragment;
            }
        }

        public override int GetServiceProviderHashCode()
        {
            if (_serviceProviderHash == null)
            {
                var hashCode = new HashCode();
                hashCode.Add(Extension._softDeleteOptions);

                _serviceProviderHash = hashCode.ToHashCode();
            }

            return _serviceProviderHash.Value;
        }

        public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
        {
            if (Extension._softDeleteOptions.Enabled)
            {
                debugInfo[$"MetioCore:{nameof(Extension.WithSoftDelete)}"] =
                    Extension._softDeleteOptions.GetHashCode().ToString(CultureInfo.InvariantCulture);
                debugInfo[$"MetioCore:{nameof(Extension.WithRemoveForeignKey)}"] =
                    Extension._removeForeignKeyEnabled.GetHashCode().ToString(CultureInfo.InvariantCulture);
            }
        }

        public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
            => other is ExtensionInfo otherInfo
                && Extension._softDeleteOptions == otherInfo.Extension._softDeleteOptions
                && Extension._removeForeignKeyEnabled == otherInfo.Extension._removeForeignKeyEnabled;
    }
}
