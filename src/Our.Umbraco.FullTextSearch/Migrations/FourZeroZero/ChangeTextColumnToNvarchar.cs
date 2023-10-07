using Microsoft.Extensions.Logging;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Persistence.Sqlite.Services;

namespace Our.Umbraco.FullTextSearch.Migrations.FourZeroZero;

public class ChangeTextColumnToNvarchar : MigrationBase
{
    private readonly ILogger<ChangeTextColumnToNvarchar> _logger;
    public ChangeTextColumnToNvarchar(IMigrationContext context, ILogger<ChangeTextColumnToNvarchar> logger) : base(context)
    {
        _logger = logger;
    }

    protected override void Migrate()
    {
        if (SqlSyntax is SqliteSyntaxProvider)
        {
            _logger.LogInformation("This project is using SQLite, which doesn't support nvarchar(max) - skipping migration");
        }
        else if (ColumnExists("FullTextCache", "Text"))
        {
            Alter.Table("FullTextCache").AlterColumn("Text").AsCustom("nvarchar(max)").Do();
        }
    }
}
