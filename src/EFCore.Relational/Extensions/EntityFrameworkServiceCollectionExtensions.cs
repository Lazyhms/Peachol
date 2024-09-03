using System.ComponentModel;

namespace Microsoft.EntityFrameworkCore.DependencyInjection;

public static class EntityFrameworkServiceCollectionExtensions
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IServiceCollection AddEntityFrameworkCoreServices(this IServiceCollection serviceCollection)
    {
        new EntityFrameworkRelationalServicesBuilder(serviceCollection)
            .TryAdd<IConventionSetPlugin, EntityFrameworkCoreConventionSetPlugin>()
            .TryAdd<ISingletonOptions, IEntityFrameworkCoreSingletonOptions>(p => p.GetRequiredService<IEntityFrameworkCoreSingletonOptions>())
            .TryAddProviderSpecificServices(
                b => b
                    .TryAddSingleton<IEntityFrameworkCoreSingletonOptions, EntityFrameworkCoreSingletonOptions>())
            .TryAddCoreServices();

        return serviceCollection;
    }
}
