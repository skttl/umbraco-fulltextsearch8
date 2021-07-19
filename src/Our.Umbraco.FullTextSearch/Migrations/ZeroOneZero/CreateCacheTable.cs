using System;
using NPoco;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace Our.Umbraco.FullTextSearch.Migrations.ZeroOneZero
{
    public class CreateCacheTable : MigrationBase
    {
        public CreateCacheTable(IMigrationContext context) : base(context)
        {
        }

        protected override void Migrate()
        {
            // Lots of methods available in the MigrationBase class - discover with this.
            if (TableExists("FullTextCache") == false)
            {
                Create.Table<CacheTableSchema>().Do();
            }
        }

        [TableName("FullTextCache")]
        [PrimaryKey("Id", AutoIncrement = true)]
        [ExplicitColumns]
        public class CacheTableSchema
        {
            [Column("Id")]
            [PrimaryKeyColumn(AutoIncrement = true)]
            public int Id { get; set; }

            [Column("NodeId")]
            public int NodeId { get; set; }

            [Column("Culture")]
            public string Culture { get; set; }

            [Column("Text")]
            [SpecialDbType(SpecialDbTypes.NTEXT)]
            public string Text { get; set; }

            [Column("LastUpdated")]
            public DateTime LastUpdated { get; set; }
        }
    }
}
