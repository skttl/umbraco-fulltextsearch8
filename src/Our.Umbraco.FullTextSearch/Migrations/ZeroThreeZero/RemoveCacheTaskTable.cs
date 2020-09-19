using System;
using Umbraco.Core.Migrations;
using Umbraco.Core.Logging;
using NPoco;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Our.Umbraco.FullTextSearch.Migrations.ZeroThreeZero
{
    public class RemoveCacheTaskTable : MigrationBase
    {
        public RemoveCacheTaskTable(IMigrationContext context) : base(context)
        {
        }

        public override void Migrate()
        {
            Logger.Debug<RemoveCacheTaskTable>("Running migration {MigrationStep}", "RemoveCacheTable");

            // Lots of methods available in the MigrationBase class - discover with this.
            if (TableExists("FullTextCacheTasks"))
            {
                Delete.Table("FullTextCacheTasks").Do();
            }
            else
            {
                Logger.Debug<RemoveCacheTaskTable>("The database table {DbTable} doesn't exist, skipping", "FullTextCacheTasks");
            }
        }
    }
}
