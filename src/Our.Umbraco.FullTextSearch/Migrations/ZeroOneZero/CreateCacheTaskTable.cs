using NPoco;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace Our.Umbraco.FullTextSearch.Migrations.ZeroOneZero
{
    public class CreateCacheTaskTable : MigrationBase
    {
        public CreateCacheTaskTable(IMigrationContext context) : base(context)
        {
        }

        protected override void Migrate()
        {
            if (TableExists("FullTextCacheTasks") == false)
            {
                Create.Table<CacheTaskTableSchema>().Do();
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
