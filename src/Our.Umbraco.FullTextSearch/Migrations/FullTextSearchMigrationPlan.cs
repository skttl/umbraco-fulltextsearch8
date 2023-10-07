using Umbraco.Cms.Infrastructure.Migrations;

namespace Our.Umbraco.FullTextSearch.Migrations
{
    public class FullTextSearchMigrationPlan : MigrationPlan
    {
        public FullTextSearchMigrationPlan()
            : base("FullTextSearch")
        {
            From(string.Empty) // nothing installed.
                .To<ZeroOneZero.CreateCacheTable>("CreateCacheTable")
                .To<ZeroOneZero.CreateCacheTaskTable>("CreateCacheTaskTable")
                .To<ZeroThreeZero.RemoveCacheTaskTable>("RemoveCacheTaskTable")
                .To<FourZeroZero.ChangeTextColumnToNvarchar>("ChangeTextColumnToNvarchar")
                .To<FourZeroZero.ReindexEverything>("ReindexEverything");
        }
    }
}
