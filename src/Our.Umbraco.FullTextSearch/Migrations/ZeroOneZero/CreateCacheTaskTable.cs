using System;
using Umbraco.Core.Migrations;
using Umbraco.Core.Logging;
using NPoco;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Our.Umbraco.FullTextSearch.Migrations.ZeroOneZero
{
    public class CreateCacheTaskTable : MigrationBase
    {
        public CreateCacheTaskTable(IMigrationContext context) : base(context)
        {
        }

        public override void Migrate()
        {
            Logger.Debug<CreateCacheTaskTable>("Running migration {MigrationStep}", "CreateCacheTable");

            // Lots of methods available in the MigrationBase class - discover with this.
            if (TableExists("FullTextCacheTasks") == false)
            {
                Create.Table<CacheTaskTableSchema>().Do();
            }
            else
            {
                Logger.Debug<CreateCacheTaskTable>("The database table {DbTable} already exists, skipping", "FullTextCacheTasks");
            }
        }

        [TableName("FullTextCacheTasks")]
        [PrimaryKey("Id", AutoIncrement = true)]
        [ExplicitColumns]
        public class CacheTaskTableSchema
        {
            [Column("Id")]
            [PrimaryKeyColumn(AutoIncrement = true)]
            public int Id { get; set; }

            [Column("NodeId")]
            public int NodeId { get; set; }

            [Column("Started")]
            public bool Started { get; set; }
        }
    }
}
