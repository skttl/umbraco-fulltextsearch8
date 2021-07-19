using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                .To<ZeroThreeZero.RemoveCacheTaskTable>("RemoveCacheTaskTable");
        }
    }
}
