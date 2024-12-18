namespace Microsoft.EntityFrameworkCore.Query;

public class QueryTranslationPreprocessorFactory<TInnerQueryTranslationPreprocessorFactory>(
    QueryTranslationPreprocessorDependencies dependencies,
    TInnerQueryTranslationPreprocessorFactory innerQueryTranslationPreprocessorFactory)
        : IQueryTranslationPreprocessorFactory
            where TInnerQueryTranslationPreprocessorFactory : IQueryTranslationPreprocessorFactory
{
    public QueryTranslationPreprocessor Create(QueryCompilationContext queryCompilationContext)
        => new QueryFilterQueryTranslationPreprocessor(
            dependencies,
            queryCompilationContext,
            innerQueryTranslationPreprocessorFactory.Create(queryCompilationContext));
}
