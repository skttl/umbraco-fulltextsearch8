using Umbraco.Cms.Infrastructure.Migrations;

namespace Our.Umbraco.FullTextSearch.Migrations;

public class FullTextSearchMigrationPlan : MigrationPlan
{
    public FullTextSearchMigrationPlan()
        : base("FullTextSearch")
    {
        From(string.Empty) // nothing installed.
            .To<FourZeroZero.CreateCacheTable>("e0f7f174-7cbf-46cd-b8d2-d59d87422d19")
            .To<FourZeroZero.ReindexEverything>("ReindexEverything");
    }
}
