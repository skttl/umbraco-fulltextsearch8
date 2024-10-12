using Examine;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Our.Umbraco.FullTextSearch.Interfaces;
using Our.Umbraco.FullTextSearch.Options;
using System;
using System.Threading.Tasks;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Services.Changes;
using Umbraco.Cms.Core.Sync;
using Umbraco.Extensions;

namespace Our.Umbraco.FullTextSearch.NotificationHandlers;

public class UpdateCacheOnPublish : INotificationHandler<ContentCacheRefresherNotification>
{
    private FullTextSearchOptions _options;
    private ILogger<UpdateCacheOnPublish> _logger;
    private IExamineManager _examineManager;
    private ICacheService _cacheService;
    private IContentService _contentService;

    public UpdateCacheOnPublish(
        IOptions<FullTextSearchOptions> options,
        ILogger<UpdateCacheOnPublish> logger,
        IExamineManager examineManager,
        ICacheService cacheService,
        IContentService contentService
        )
    {
        _options = options.Value;
        _logger = logger;
        _examineManager = examineManager;
        _cacheService = cacheService;
        _contentService = contentService;

    }

    public void Handle(ContentCacheRefresherNotification notification)
    {

        if (notification.MessageType != MessageType.RefreshByPayload)
            return;

        if (!_options.Enabled)
        {
            _logger.LogDebug("FullTextIndexing is not enabled");
            return;
        }

        if (!_examineManager.TryGetIndex(Constants.UmbracoIndexes.ExternalIndexName, out IIndex index))
        {
            _logger.LogError(new InvalidOperationException($"No index found by name {Constants.UmbracoIndexes.ExternalIndexName}"), $"No index found by name {Constants.UmbracoIndexes.ExternalIndexName}");
            return;
        }

        if (_cacheService.CacheTableExists() == false)
        {
            _logger.LogError("Cache table doesn't exist, maybe the site is installing");
            return;
        }

        foreach (var payload in (ContentCacheRefresher.JsonPayload[])notification.MessageObject)
        {
            if (payload.ChangeTypes.HasType(TreeChangeTypes.Remove))
            {
                Task.WaitAll(_cacheService.DeleteFromCache(payload.Id));
            }
            else if (payload.ChangeTypes.HasType(TreeChangeTypes.RefreshAll))
            {
                // just ignore that payload (Umbracos examine implementation does the same)
            }
            else // RefreshNode or RefreshBranch (maybe trashed)
            {
                Task.WaitAll(_cacheService.AddToCache(payload.Id));

                // branch
                if (payload.ChangeTypes.HasType(TreeChangeTypes.RefreshBranch))
                {
                    const int pageSize = 500;
                    var page = 0;
                    var total = long.MaxValue;
                    while (page * pageSize < total)
                    {
                        var descendants = _contentService.GetPagedDescendants(payload.Id, page++, pageSize, out total,
                            //order by shallowest to deepest, this allows us to check it's published state without checking every item
                            ordering: Ordering.By("Path", Direction.Ascending));

                        foreach (var descendant in descendants)
                        {
                            Task.WaitAll(_cacheService.AddToCache(descendant.Id));
                        }
                    }
                }
            }
        }
    }


}
