using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Migrations;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade;
using Umbraco.Cms.Infrastructure.Scoping;

namespace Our.Umbraco.FullTextSearch.Migrations;

public class ExecuteMigrations(
    IScopeProvider scopeProvider,
    IMigrationPlanExecutor migrationPlanExecutor,
    IKeyValueService keyValueService)
    : INotificationHandler<UmbracoApplicationStartingNotification>
{
    public void Handle(UmbracoApplicationStartingNotification notification)
    {
        if (notification.RuntimeLevel >= RuntimeLevel.Run)
        {
            // register and run our migration plan
            var upgrader = new Upgrader(new FullTextSearchMigrationPlan());
            upgrader.ExecuteAsync(migrationPlanExecutor, scopeProvider, keyValueService);
        }
    }
}
