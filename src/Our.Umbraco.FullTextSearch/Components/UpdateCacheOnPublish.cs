using Examine;
using Our.Umbraco.FullTextSearch.Interfaces;
using System;
using Umbraco.Core.Cache;
using Umbraco.Core.Composing;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence.DatabaseModelDefinitions;
using Umbraco.Core.Services;
using Umbraco.Core.Services.Changes;
using Umbraco.Core.Sync;
using Umbraco.Web.Cache;

namespace Our.Umbraco.FullTextSearch.Components
{
    public class UpdateCacheOnPublishComposer : ComponentComposer<UpdateCacheOnPublish>
    {
    }
    public class UpdateCacheOnPublish : IComponent
    {
        private readonly ICacheService _cacheService;
        private readonly IConfig _fullTextConfig;
        private readonly ILogger _logger;
        private readonly IExamineManager _examineManager;
        private readonly IContentService _contentService;

        public UpdateCacheOnPublish(ICacheService cacheService,
            ILogger logger,
            IConfig fullTextConfig,
            IExamineManager examineManager,
            IContentService contentService)
        {
            _cacheService = cacheService;
            _fullTextConfig = fullTextConfig;
            _logger = logger;
            _examineManager = examineManager;
            _contentService = contentService;
        }
        public void Initialize()
        {
            ContentCacheRefresher.CacheUpdated += ContentCacheRefresher_CacheUpdated;
        }

        private void ContentCacheRefresher_CacheUpdated(ContentCacheRefresher sender, CacheRefresherEventArgs args)
        {
            if (args.MessageType != MessageType.RefreshByPayload)
                return;

            if (!_fullTextConfig.IsFullTextIndexingEnabled())
            {
                _logger.Debug<UpdateCacheOnPublish>("FullTextIndexing is not enabled");
                return;
            }

            if (!_examineManager.TryGetIndex("ExternalIndex", out IIndex index))
            {
                _logger.Error<UpdateCacheOnPublish>(new InvalidOperationException("No index found by name ExternalIndex"));
                return;
            }

            foreach (var payload in (ContentCacheRefresher.JsonPayload[])args.MessageObject)
            {

                if (payload.ChangeTypes.HasType(TreeChangeTypes.Remove))
                {
                    _cacheService.DeleteFromCache(payload.Id);
                }
                else if (payload.ChangeTypes.HasType(TreeChangeTypes.RefreshAll))
                {
                    // just ignore that payload (Umbracos examine implementation does the same)
                }
                else // RefreshNode or RefreshBranch (maybe trashed)
                {
                    _cacheService.AddCacheTask(payload.Id);

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
                                _cacheService.AddCacheTask(descendant.Id);
                            }
                        }
                    }
                }
            }
        }

        public void Terminate()
        {
        }
    }
}
