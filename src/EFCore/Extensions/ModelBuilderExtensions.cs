namespace Microsoft.EntityFrameworkCore;

public static class ModelBuilderExtensions
{
    public static ModelBuilder ApplyEntitiesFromAssembly(this ModelBuilder builder, string assemblyName)
        => builder.ApplyEntitiesFromAssembly(Assembly.Load(assemblyName));

    public static ModelBuilder ApplyEntitiesFromAssembly<TBaseEntity>(this ModelBuilder builder, string assemblyName) where TBaseEntity : class
        => builder.ApplyEntitiesFromAssembly<TBaseEntity>(Assembly.Load(assemblyName));

    public static ModelBuilder ApplyEntitiesFromAssembly<TBaseEntity>(this ModelBuilder builder, Assembly assembly, params Expression<Func<TBaseEntity, bool>>[] baseQueryFilters) where TBaseEntity : class
        => builder.ApplyEntitiesFromAssembly(assembly, typeof(TBaseEntity), baseQueryFilters);

    public static ModelBuilder ApplyEntitiesFromAssembly<TBaseEntity>(this ModelBuilder builder, Assembly assembly, Type baseType, params Expression<Func<TBaseEntity, bool>>[] baseQueryFilters) where TBaseEntity : class
        => builder.ApplyEntitiesFromAssembly(assembly, w => baseType.IsAssignableFrom(w) && w.IsClass && !w.IsAbstract || w.IsDefined<DbEntityAttribute>(), baseQueryFilters);

    public static ModelBuilder ApplyEntitiesFromAssembly(this ModelBuilder builder, Assembly assembly, Func<Type, bool>? predicate = null, params LambdaExpression[] baseQueryFilters)
    {
        IEnumerable<Type> assemblyTypes = assembly.GetTypes();
        if (null != predicate)
        {
            assemblyTypes = assemblyTypes.Where(predicate);
        }
        return builder.ApplyEntitiesFromAssembly(assemblyTypes, baseQueryFilters);
    }

    public static ModelBuilder ApplyEntitiesFromAssembly(this ModelBuilder builder, IEnumerable<Type> types, params LambdaExpression[] baseQueryFilters)
    {
        foreach (var type in types)
        {
            var entityTypeBuilder = builder.Entity(type);
            if (baseQueryFilters != null && baseQueryFilters.Length > 0)
            {
                var filter = baseQueryFilters.Select(baseQueryFilter =>
                {
                    var parameterExpression = Expression.Parameter(type, baseQueryFilter.Parameters[0].Name);
                    var expressionFilter = ReplacingExpressionVisitor.Replace(
                        baseQueryFilter.Parameters[0], parameterExpression, baseQueryFilter.Body);
                    return Expression.Lambda(expressionFilter, parameterExpression);
                });
                entityTypeBuilder.HasStoredQueryFilter(filter);
            }
        }
        return builder;
    }
}