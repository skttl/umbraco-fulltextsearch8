using NPoco;
using System;
using System.Threading.Tasks;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace Our.Umbraco.FullTextSearch.Migrations.FourZeroZero;

public class CreateCacheTable : AsyncMigrationBase
{
    public CreateCacheTable(IMigrationContext context) : base(context)
    {
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
        [SpecialDbType(SpecialDbTypes.NVARCHARMAX)]
        public string Text { get; set; }

        [Column("LastUpdated")]
        public DateTime LastUpdated { get; set; }
    }

    protected override Task MigrateAsync()
    {
        if (TableExists("FullTextCache") == false)
        {
            Create.Table<CacheTableSchema>().Do();
        }
        return Task.CompletedTask;
    }
}
