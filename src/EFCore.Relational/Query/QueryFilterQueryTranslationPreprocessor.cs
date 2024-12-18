using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Microsoft.EntityFrameworkCore.Query;

public class QueryFilterQueryTranslationPreprocessor(
    QueryTranslationPreprocessorDependencies dependencies,
    QueryCompilationContext queryCompilationContext,
    QueryTranslationPreprocessor innerQueryTranslationPreprocessor)
        : QueryTranslationPreprocessor(dependencies, queryCompilationContext)
{
    private readonly Parameters _parameters = new();

    public override Expression Process(Expression query)
    {
        query = new QueryFilterExpressionVisitor(QueryCompilationContext, Dependencies.EvaluatableExpressionFilter, _parameters).Visit(query);

        var dbContextOnQueryContextPropertyAccess =
            Expression.Convert(
                Expression.Property(
                    QueryCompilationContext.QueryContextParameter,
                    typeof(QueryContext).GetTypeInfo().GetDeclaredProperty(nameof(QueryContext.Context))!),
                QueryCompilationContext.ContextType);

        foreach (var (key, value) in _parameters.ParameterValues)
        {
            var lambda = (LambdaExpression)value!;
            var remappedLambdaBody = ReplacingExpressionVisitor.Replace(
                lambda.Parameters[0],
                dbContextOnQueryContextPropertyAccess,
                lambda.Body);

            QueryCompilationContext.RegisterRuntimeParameter(
                key,
                Expression.Lambda(
                    remappedLambdaBody.Type.IsValueType
                        ? Expression.Convert(remappedLambdaBody, typeof(object))
                        : remappedLambdaBody,
                    QueryCompilationContext.QueryContextParameter));
        }

        return innerQueryTranslationPreprocessor.Process(query);
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
