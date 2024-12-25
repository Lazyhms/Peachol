namespace Microsoft.EntityFrameworkCore.Metadata.Conventions;

public class PeacholConventionSetPlugin(
    IPeacholSingletonOptions peacholSingletonOptions,
    ProviderConventionSetBuilderDependencies dependencies) : IConventionSetPlugin
{
    protected IPeacholSingletonOptions PeacholSingletonOptions { get; } = peacholSingletonOptions;

    protected ProviderConventionSetBuilderDependencies Dependencies { get; } = dependencies;

    public virtual ConventionSet ModifyConventions(ConventionSet conventionSet)
    {
        if (PeacholSingletonOptions.RemoveForeignKeyEnabled)
        {
            conventionSet.Remove(typeof(ForeignKeyIndexConvention));
        }

        var columnDefaultValueConvention = new ColumnDefaultValueConvention(Dependencies);
        conventionSet.PropertyAddedConventions.Add(columnDefaultValueConvention);
        conventionSet.PropertyFieldChangedConventions.Add(columnDefaultValueConvention);

        var columnDefaultValueSqlConvention = new ColumnDefaultValueSqlConvention(Dependencies);
        conventionSet.PropertyAddedConventions.Add(columnDefaultValueSqlConvention);
        conventionSet.PropertyFieldChangedConventions.Add(columnDefaultValueSqlConvention);

        var columnUpdateIgnoreConvention = new ColumnUpdateIgnoreConvention(Dependencies);
        conventionSet.PropertyAddedConventions.Add(columnUpdateIgnoreConvention);
        conventionSet.PropertyFieldChangedConventions.Add(columnUpdateIgnoreConvention);

        var columnInsertIgnoreConvention = new ColumnAddIgnoreConvention(Dependencies);
        conventionSet.PropertyAddedConventions.Add(columnInsertIgnoreConvention);
        conventionSet.PropertyFieldChangedConventions.Add(columnInsertIgnoreConvention);

        var tableAndColumnCommentConvention = new TableAndColumnCommentConvention(PeacholSingletonOptions);
        conventionSet.ModelFinalizingConventions.Add(tableAndColumnCommentConvention);

        return conventionSet;
    }
}