using Umbraco.Cms.Infrastructure.Migrations;

namespace Our.Umbraco.FullTextSearch.Migrations.FourZeroZero;

public class ChangeTextColumnToNvarchar : MigrationBase
{
    public ChangeTextColumnToNvarchar(IMigrationContext context) : base(context)
    {
    }

    protected override void Migrate()
    {
        if (ColumnExists("FullTextCache", "Text"))
        {
            Alter.Table("FullTextCache").AlterColumn("Text").AsCustom("nvarchar(max)").Do();
        }
    }
}
