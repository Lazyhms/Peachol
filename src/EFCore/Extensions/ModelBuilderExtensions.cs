namespace Microsoft.EntityFrameworkCore;

public static class ModelBuilderExtensions
{
    public static ModelBuilder ApplyEntitiesFromAssembly(this ModelBuilder builder, string assemblyName)
        => builder.ApplyEntitiesFromAssembly(Assembly.Load(assemblyName));

    public static ModelBuilder ApplyEntitiesFromAssembly(this ModelBuilder builder, Assembly assembly)
        => builder.ApplyEntitiesFromAssembly(assembly, w => w.IsDefined<DbEntityAttribute>());

    public static ModelBuilder ApplyEntitiesFromAssembly(this ModelBuilder builder, Assembly assembly, Func<Type, bool>? predicate = null)
    {
        IEnumerable<Type> types = assembly.GetTypes();
        if (null != predicate)
        {
            types = types.Where(predicate);
        }
        foreach (var type in types)
        {
            builder.Entity(type);
        }
        return builder;
    }
}