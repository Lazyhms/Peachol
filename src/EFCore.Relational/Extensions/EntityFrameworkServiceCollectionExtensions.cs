using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using System.ComponentModel;

namespace Microsoft.EntityFrameworkCore.DependencyInjection;

public static class EntityFrameworkServiceCollectionExtensions
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IServiceCollection AddEntityFrameworkCoreServices(this IServiceCollection serviceCollection)
    {
        new EntityFrameworkRelationalServicesBuilder(serviceCollection)
            .TryAdd<IConventionSetPlugin, PeacholConventionSetPlugin>()
            .TryAdd<ISingletonOptions, IPeacholSingletonOptions>(p => p.GetRequiredService<IPeacholSingletonOptions>())
            .TryAddProviderSpecificServices(
                b => b
                    .TryAddSingleton<IPeacholSingletonOptions, PeacholSingletonOptions>());

        return serviceCollection;
    }
}
