namespace Microsoft.EntityFrameworkCore.Metadata.Conventions;

public class PeacholConventionSetPlugin(
    IPeacholSingletonOptions entityFrameworkCoreSingletonOptions,
    ProviderConventionSetBuilderDependencies dependencies) : IConventionSetPlugin
{
    protected IPeacholSingletonOptions EntityFrameworkCoreSingletonOptions { get; }
        = entityFrameworkCoreSingletonOptions;

    protected ProviderConventionSetBuilderDependencies Dependencies { get; } = dependencies;

    public virtual ConventionSet ModifyConventions(ConventionSet conventionSet)
    {
        if (EntityFrameworkCoreSingletonOptions.RemoveForeignKeyEnabled)
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

        var metioModelFinalizingConvention = new TableAndColumnCommentConvention(EntityFrameworkCoreSingletonOptions);
        conventionSet.ModelFinalizingConventions.Add(metioModelFinalizingConvention);

        return conventionSet;
    }
}