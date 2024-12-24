using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Microsoft.EntityFrameworkCore.Query;

public class QueryFilterExpressionVisitor : ExpressionVisitor
{
    private bool _ignoredQueryFilter;
    private readonly Parameters _parameters;
    private readonly IList<string> _ignoredQueryFilterNames;

    private readonly MethodInfo _ignoreQueryFiltersMethodInfo =
        typeof(EntityFrameworkQueryableExtensions).GetTypeInfo().GetDeclaredMethod(nameof(EntityFrameworkQueryableExtensions.IgnoreQueryFilters))!;

    private readonly QueryCompilationContext _queryCompilationContext;

#pragma warning disable EF1001 // Internal EF Core API usage.
    private readonly ParameterExtractingExpressionVisitor _parameterExtractingExpressionVisitor;
#pragma warning restore EF1001 // Internal EF Core API usage.

    public QueryFilterExpressionVisitor(QueryCompilationContext queryCompilationContext, IEvaluatableExpressionFilter evaluatableExpressionFilter)
    {
        _parameters = new();
        _ignoredQueryFilter = false;
        _ignoredQueryFilterNames = [];

        _queryCompilationContext = queryCompilationContext;

#pragma warning disable EF1001 // Internal EF Core API usage.
        _parameterExtractingExpressionVisitor = new(
            evaluatableExpressionFilter,
            _parameters,
            queryCompilationContext.ContextType,
            queryCompilationContext.Model,
            queryCompilationContext.Logger,
            false,
            true);
#pragma warning restore EF1001 // Internal EF Core API usage.
    }

    public Expression ApplyStoredQueryFilter(Expression query)
    {
        var queryFilter = Visit(query);

        var dbContextOnQueryContextPropertyAccess =
            Expression.Convert(
                Expression.Property(
                    QueryCompilationContext.QueryContextParameter,
                    typeof(QueryContext).GetTypeInfo().GetDeclaredProperty(nameof(QueryContext.Context))!),
                _queryCompilationContext.ContextType);

        foreach (var (key, value) in _parameters.ParameterValues)
        {
            var lambda = (LambdaExpression)value!;
            var remappedLambdaBody = ReplacingExpressionVisitor.Replace(
                lambda.Parameters[0],
                dbContextOnQueryContextPropertyAccess,
                lambda.Body);

            _queryCompilationContext.RegisterRuntimeParameter(
                key,
                Expression.Lambda(
                    remappedLambdaBody.Type.IsValueType
                        ? Expression.Convert(remappedLambdaBody, typeof(object))
                        : remappedLambdaBody,
                    QueryCompilationContext.QueryContextParameter));
        }

        return queryFilter;
    }

    protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression)
    {
        var genericMethodDefinition = methodCallExpression.Method.GetGenericMethodDefinition();

        if (genericMethodDefinition == _ignoreQueryFiltersMethodInfo)
        {
            _ignoredQueryFilter = true;
            return base.VisitMethodCall(methodCallExpression);
        }
        if (genericMethodDefinition == RelationalEntityFrameworkCoreQueryableExtensions.StringIgnoreQueryFiltersMethodInfo)
        {
            foreach (var item in (IEnumerable<string>)((ConstantExpression)methodCallExpression.Arguments[1]).Value!)
            {
                _ignoredQueryFilterNames.Add(item);
            }
            return base.Visit(methodCallExpression.Arguments[0]);
        }

        return base.VisitMethodCall(methodCallExpression);
    }

    protected override Expression VisitExtension(Expression expression)
    {
        if (expression is EntityQueryRootExpression entityQueryRootExpression
            && entityQueryRootExpression.EntityType.GetStoredQueryFilter() is { } storedQueryFilter)
        {
            Expression queryRootExpression = entityQueryRootExpression;
            foreach (var queryFilter in storedQueryFilter)
            {
                if (_ignoredQueryFilter ||
                    _ignoredQueryFilterNames.Contains(queryFilter.Key))
                {
                    continue;
                }

#pragma warning disable EF1001 // Internal EF Core API usage.
                var extractExpression = (LambdaExpression)_parameterExtractingExpressionVisitor.ExtractParameters(queryFilter.Value, false);
#pragma warning restore EF1001 // Internal EF Core API usage.

                queryRootExpression = Expression.Call(
                    method: QueryableMethods.Where.MakeGenericMethod(entityQueryRootExpression.EntityType.ClrType),
                    arg0: queryRootExpression ?? entityQueryRootExpression,
                    arg1: extractExpression);
            }
            return queryRootExpression;
        }

        return base.VisitExtension(expression);
    }

#pragma warning disable EF1001 // Internal EF Core API usage.
    private sealed class Parameters : IParameterValues
#pragma warning restore EF1001 // Internal EF Core API usage.
    {
        private readonly Dictionary<string, object?> _parameterValues = [];

        public IReadOnlyDictionary<string, object?> ParameterValues
            => _parameterValues;

        public void AddParameter(string name, object? value)
            => _parameterValues.Add(name, value);
    }
}