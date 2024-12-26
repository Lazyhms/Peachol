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
#if NET8_0
    private readonly ParameterExtractingExpressionVisitor _parameterExtractingExpressionVisitor;
#elif NET9_0
    private readonly ExpressionTreeFuncletizer _expressionTreeFuncletizer;
#endif
#pragma warning restore EF1001 // Internal EF Core API usage.

    public QueryFilterExpressionVisitor(QueryCompilationContext queryCompilationContext, IEvaluatableExpressionFilter evaluatableExpressionFilter)
    {
        _parameters = new();
        _ignoredQueryFilter = false;
        _ignoredQueryFilterNames = [];

        _queryCompilationContext = queryCompilationContext;

#pragma warning disable EF1001 // Internal EF Core API usage.
#if NET8_0
        _parameterExtractingExpressionVisitor = new(evaluatableExpressionFilter, _parameters, queryCompilationContext.ContextType, queryCompilationContext.Model, queryCompilationContext.Logger, false, true);
#elif NET9_0
        _expressionTreeFuncletizer = new ExpressionTreeFuncletizer(_queryCompilationContext.Model, evaluatableExpressionFilter, _queryCompilationContext.ContextType, generateContextAccessors: true, _queryCompilationContext.Logger);
#endif
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
#if NET8_0
                var extractExpression = (LambdaExpression)_parameterExtractingExpressionVisitor.ExtractParameters(queryFilter.Value, false);
#elif NET9_0
#pragma warning disable EF9100 // 类型仅用于评估，在将来的更新中可能会被更改或删除。取消此诊断以继续。
                var extractExpression = (LambdaExpression)_expressionTreeFuncletizer.ExtractParameters(queryFilter.Value.Body, _parameters, false, false, _queryCompilationContext.IsPrecompiling, out IReadOnlySet<string> _);
#pragma warning restore EF9100 // 类型仅用于评估，在将来的更新中可能会被更改或删除。取消此诊断以继续。
#endif
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