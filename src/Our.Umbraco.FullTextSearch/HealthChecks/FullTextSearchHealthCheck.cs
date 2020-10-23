using Examine;
using Newtonsoft.Json.Linq;
using Our.Umbraco.FullTextSearch.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Services;
using Umbraco.Examine;
using Umbraco.Web.HealthCheck;

namespace Our.Umbraco.FullTextSearch.HealthChecks
{
    [HealthCheck(
        "98074970-9032-4db4-b517-de18d4da7057",
        "FullTextSearch",
        Description = "Health Checks for FullTextSearch indexes",
        Group = "Search"
        )]
    public class FullTextSearchHealthCheck : HealthCheck
    {
        private readonly IStatusService _statusService;
        private readonly ICacheService _cacheService;
        private readonly IFullTextSearchConfig _fullTextSearchConfig;
        private readonly ILocalizedTextService _textService;
        private readonly IPublishedContentValueSetBuilder _valueSetBuilder;
        private readonly IContentService _contentService;
        private readonly IExamineManager _examineManager;

        public FullTextSearchHealthCheck(
            IStatusService statusService,
            ICacheService cacheService,
            IFullTextSearchConfig fullTextSearchConfig,
            ILocalizedTextService textService,
            IPublishedContentValueSetBuilder valueSetBuilder,
            IContentService contentService,
            IExamineManager examineManager)
        {
            _statusService = statusService;
            _cacheService = cacheService;
            _fullTextSearchConfig = fullTextSearchConfig;
            _textService = textService;
            _valueSetBuilder = valueSetBuilder;
            _contentService = contentService;
            _examineManager = examineManager;
        }
        public override HealthCheckStatus ExecuteAction(HealthCheckAction action)
        {
            if (action.Alias == "reindexNodes")
            {
                if (action.ActionParameters["nodeIds"] is JArray nodeIds)
                {
                    if (!_examineManager.TryGetIndex("ExternalIndex", out IIndex index))
                    {
                        return new HealthCheckStatus(_textService.Localize("fullTextSearch/externalIndexNotFound"))
                        {
                            ResultType = StatusResultType.Error
                        };
                    }

                    foreach (int id in nodeIds)
                    {
                        _cacheService.AddToCache(id);
                    }
                    index.IndexItems(_valueSetBuilder.GetValueSets(_contentService.GetByIds(nodeIds.Select(x => x.Value<int>())).ToArray()));
                    var textFormat = _textService.Localize("fullTextSearch/reindexedNodes");
                    return new HealthCheckStatus(string.Format(textFormat, nodeIds.Count))
                    {
                        ResultType = StatusResultType.Success
                    };
                }
                else
                {
                    throw new ArgumentException("Please provide nodeids to be reindexed");
                }
            }
            throw new NotImplementedException($"{action.Alias} action is not implemented");
        }

        public override IEnumerable<HealthCheckStatus> GetStatus()
        {
            if (!_fullTextSearchConfig.Enabled)
            {
                return new[]
                {
                    new HealthCheckStatus(_textService.Localize("fullTextSearch/fullTextSearchIsDisabled"))
                    {
                        ResultType = StatusResultType.Warning
                    }
                };
            }

            return new[]
            {
                GetMissingNodesStatus(),
                GetIncorrectIndexedNodesStatus()
            };
        }

        private HealthCheckStatus GetMissingNodesStatus()
        {
            if (!_statusService.TryGetMissingNodes(out ISearchResults missingNodes))
            {
                return new HealthCheckStatus(_textService.Localize("fullTextSearch/couldntGetMissingNodes"))
                {
                    ResultType = StatusResultType.Error
                };
            }

            if (missingNodes.TotalItemCount > 0)
            {
                return new HealthCheckStatus(string.Format(_textService.Localize("fullTextSearch/nodesAreMissingInIndex"), missingNodes.TotalItemCount))
                {
                    ResultType = StatusResultType.Error,
                    Actions = new[] { ReIndexNodes(missingNodes) }
                };
            }

            return new HealthCheckStatus(_textService.Localize("fullTextSearch/allIndexableNodesAreIndexed"))
            {
                ResultType = StatusResultType.Success
            };
        }

        private HealthCheckStatus GetIncorrectIndexedNodesStatus()
        {
            if (!_statusService.TryGetIncorrectIndexedNodes(out ISearchResults incorrectIndexedNodes))
            {
                return new HealthCheckStatus(_textService.Localize("fullTextSearch/couldntGetIncorrectIndexedNodes"))
                {
                    ResultType = StatusResultType.Error
                };
            }

            if (incorrectIndexedNodes != null && incorrectIndexedNodes.TotalItemCount > 0)
            {

                return new HealthCheckStatus(string.Format(_textService.Localize("fullTextSearch/nodesAreIncorrectlyIndexed"), incorrectIndexedNodes.TotalItemCount))
                {
                    ResultType = StatusResultType.Error,
                    Actions = new[] { ReIndexNodes(incorrectIndexedNodes) }
                };
            }

            return new HealthCheckStatus(string.Format(_textService.Localize("fullTextSearch/nodesAreIncorrectlyIndexed"), 0))
            {
                ResultType = StatusResultType.Success
            };
        }

        private HealthCheckAction ReIndexNodes(ISearchResults nodes)
        {

            var reindexNodesAction = new HealthCheckAction("reindexNodes", Id)
            {
                Name = _textService.Localize("fullTextSearch/reindexNodes")
            };
            if (nodes != null && nodes.TotalItemCount > 0)
            {
                var nodeIds = nodes.Select(x => int.TryParse(x.Id, out int id) ? id : 0).Where(x => x != 0);
                reindexNodesAction.ActionParameters = new Dictionary<string, object>() {
                    { "nodeIds", nodeIds }
                };
            }

            return reindexNodesAction;
        }
    }
}
