using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Microsoft.EntityFrameworkCore.Query;
#pragma warning disable EF1001 // Internal EF Core API usage.
public class QueryFilterExpressionVisitor(QueryCompilationContext queryCompilationContext, IEvaluatableExpressionFilter evaluatableExpressionFilter, IParameterValues parameterValues) : ExpressionVisitor
{
    private readonly ParameterExtractingExpressionVisitor _parameterExtractingExpressionVisitor =
            new(evaluatableExpressionFilter, parameterValues, queryCompilationContext.ContextType, queryCompilationContext.Model, queryCompilationContext.Logger, false, true);
#pragma warning restore EF1001 // Internal EF Core API usage.

    private bool _ignoredQueryFilter = false;
    private readonly IList<string> _ignoredQueryFilterNames = [];

    private readonly MethodInfo _ignoreQueryFiltersMethodInfo =
        typeof(EntityFrameworkQueryableExtensions).GetTypeInfo().GetDeclaredMethod(nameof(EntityFrameworkQueryableExtensions.IgnoreQueryFilters))!;

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
            foreach (var item in (string[])((ConstantExpression)methodCallExpression.Arguments[1]).Value!)
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
            && entityQueryRootExpression.EntityType.GetStoredQueryFilter() is { } namedQueryFilter)
        {
            Expression queryRootExpression = entityQueryRootExpression;
            foreach (var queryFilter in namedQueryFilter)
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
}