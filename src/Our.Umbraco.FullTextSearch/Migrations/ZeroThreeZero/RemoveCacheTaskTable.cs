using Umbraco.Cms.Infrastructure.Migrations;

namespace Our.Umbraco.FullTextSearch.Migrations.ZeroThreeZero
{
    public class RemoveCacheTaskTable : MigrationBase
    {
        public RemoveCacheTaskTable(IMigrationContext context) : base(context)
        {
        }

        protected override void Migrate()
        {
            if (TableExists("FullTextCacheTasks"))
            {
                Delete.Table("FullTextCacheTasks").Do();
            }
        }
    }
}
