using Examine;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Our.Umbraco.FullTextSearch.Interfaces;
using Our.Umbraco.FullTextSearch.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.HealthChecks;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Examine;
using Umbraco.Extensions;

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
        private readonly FullTextSearchOptions _options;
        private readonly ILocalizedTextService _textService;
        private readonly IPublishedContentValueSetBuilder _valueSetBuilder;
        private readonly IContentService _contentService;
        private readonly IExamineManager _examineManager;

        public FullTextSearchHealthCheck(
            IStatusService statusService,
            ICacheService cacheService,
            IOptions<FullTextSearchOptions> options,
            ILocalizedTextService textService,
            IPublishedContentValueSetBuilder valueSetBuilder,
            IContentService contentService,
            IExamineManager examineManager)
        {
            _statusService = statusService;
            _cacheService = cacheService;
            _options = options.Value;
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
                    if (!_examineManager.TryGetIndex(Constants.UmbracoIndexes.ExternalIndexName, out IIndex index))
                    {
                        return new HealthCheckStatus(_textService.Localize("fullTextSearch","externalIndexNotFound"))
                        {
                            ResultType = StatusResultType.Error
                        };
                    }

                    foreach (int id in nodeIds)
                    {
                        _cacheService.AddToCache(id);
                    }
                    index.IndexItems(_valueSetBuilder.GetValueSets(_contentService.GetByIds(nodeIds.Select(x => x.Value<int>())).ToArray()));
                    var textFormat = _textService.Localize("fullTextSearch", "reindexedMessage");
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

        public override Task<IEnumerable<HealthCheckStatus>> GetStatus()
        {
            if (!_options.Enabled)
            {
                return Task.FromResult(new HealthCheckStatus(_textService.Localize("fullTextSearch","fullTextSearchIsDisabled"))
                    {
                        ResultType = StatusResultType.Warning
                    }.Yield());
            }
            else
            {
                var result = new List<HealthCheckStatus>() 
                { 
                    new HealthCheckStatus(_textService.Localize("fullTextSearch", "fullTextSearchIsEnabled"))
                    {
                        ResultType = StatusResultType.Success
                    },
                    GetMissingNodesStatus(), 
                    GetIncorrectIndexedNodesStatus() 
                };
                return Task.FromResult((IEnumerable<HealthCheckStatus>)result);
            };
        }

        private HealthCheckStatus GetMissingNodesStatus()
        {
            if (!_statusService.TryGetMissingNodes(out ISearchResults missingNodes))
            {
                return new HealthCheckStatus(_textService.Localize("fullTextSearch","couldntGetMissingNodes"))
                {
                    ResultType = StatusResultType.Error
                };
            }

            if (missingNodes.TotalItemCount > 0)
            {
                return new HealthCheckStatus(string.Format(_textService.Localize("fullTextSearch","nodesAreMissingInIndex"), missingNodes.TotalItemCount))
                {
                    Description = _textService.Localize("fullTextSearch", "missingNodesDescription"),
                    ResultType = StatusResultType.Error,
                    Actions = new[] { ReIndexNodes(missingNodes) }
                };
            }

            return new HealthCheckStatus("#fullTextSearch_allIndexableNodesAreIndexed")
            {
                Description = _textService.Localize("fullTextSearch", "indexableNodesDescription"),
                ResultType = StatusResultType.Success
            };
        }

        private HealthCheckStatus GetIncorrectIndexedNodesStatus()
        {
            if (!_statusService.TryGetIncorrectIndexedNodes(out ISearchResults incorrectIndexedNodes))
            {
                return new HealthCheckStatus(_textService.Localize("fullTextSearch","couldntGetIncorrectIndexedNodes"))
                {
                    ResultType = StatusResultType.Error
                };
            }

            if (incorrectIndexedNodes != null && incorrectIndexedNodes.TotalItemCount > 0)
            {

                return new HealthCheckStatus(string.Format(_textService.Localize("fullTextSearch","nodesAreIncorrectlyIndexed"), incorrectIndexedNodes.TotalItemCount))
                {
                    Description = _textService.Localize("fullTextSearch", "incorrectIndexedNodesDescription"),
                    ResultType = StatusResultType.Error,
                    Actions = new[] { ReIndexNodes(incorrectIndexedNodes) }
                };
            }

            return new HealthCheckStatus(string.Format(_textService.Localize("fullTextSearch","nodesAreIncorrectlyIndexed"), 0))
            {
                Description = _textService.Localize("fullTextSearch", "incorrectIndexedNodesDescription"),
                ResultType = StatusResultType.Success
            };
        }

        private HealthCheckAction ReIndexNodes(ISearchResults nodes)
        {

            var reindexNodesAction = new HealthCheckAction("reindexNodes", Id)
            {
                Name = _textService.Localize("fullTextSearch","reindexNodes")
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
