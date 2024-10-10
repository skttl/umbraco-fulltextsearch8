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
        private readonly IPublishedContentValueSetBuilder _valueSetBuilder;
        private readonly IContentService _contentService;
        private readonly IExamineManager _examineManager;

        public FullTextSearchHealthCheck(
            IStatusService statusService,
            ICacheService cacheService,
            IOptions<FullTextSearchOptions> options,
            IPublishedContentValueSetBuilder valueSetBuilder,
            IContentService contentService,
            IExamineManager examineManager)
        {
            _statusService = statusService;
            _cacheService = cacheService;
            _options = options.Value;
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
                        return new HealthCheckStatus("ExternalIndex not found")
                        {
                            ResultType = StatusResultType.Error
                        };
                    }

                    foreach (int id in nodeIds)
                    {
                        _cacheService.AddToCache(id);
                    }
                    index.IndexItems(_valueSetBuilder.GetValueSets(_contentService.GetByIds(nodeIds.Select(x => x.Value<int>())).ToArray()));
                    return new HealthCheckStatus("Reindexing complete")
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
                return Task.FromResult(new HealthCheckStatus("FullTextSearch is disabled")
                    {
                        ResultType = StatusResultType.Warning
                    }.Yield());
            }
            else
            {
                var result = new List<HealthCheckStatus>() 
                { 
                    new HealthCheckStatus("FullTextSearch is enabled")
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
                return new HealthCheckStatus("Couldn't get missing nodes")
                {
                    ResultType = StatusResultType.Error
                };
            }

            if (missingNodes.TotalItemCount > 0)
            {
                return new HealthCheckStatus(string.Format("{0} node(s) are missing full text content in index", missingNodes.TotalItemCount))
                {
                    Description = "The total number of missing indexed nodes, according to the current Full Text Search config",
                    ResultType = StatusResultType.Error,
                    Actions = new[] { ReIndexNodes(missingNodes) }
                };
            }

            return new HealthCheckStatus("All indexable nodes has full text content in index")
            {
                Description = "The total number of indexable nodes, according to the current Full Text Search config",
                ResultType = StatusResultType.Success
            };
        }

        private HealthCheckStatus GetIncorrectIndexedNodesStatus()
        {
            if (!_statusService.TryGetIncorrectIndexedNodes(out ISearchResults incorrectIndexedNodes))
            {
                return new HealthCheckStatus("Couldn't get incorrectly indexed nodes")
                {
                    ResultType = StatusResultType.Error
                };
            }

            if (incorrectIndexedNodes != null && incorrectIndexedNodes.TotalItemCount > 0)
            {

                return new HealthCheckStatus(string.Format("{0} node(s) are incorrectly indexed with full text content", incorrectIndexedNodes.TotalItemCount))
                {
                    Description = "The total number of indexed nodes searchable by Full Text Search",
                    ResultType = StatusResultType.Error,
                    Actions = new[] { ReIndexNodes(incorrectIndexedNodes) }
                };
            }

            return new HealthCheckStatus(string.Format("{0} node(s) are incorrectly indexed with full text content", 0))
            {
                Description = "The total number of indexed nodes searchable by Full Text Search",
                ResultType = StatusResultType.Success
            };
        }

        private HealthCheckAction ReIndexNodes(ISearchResults nodes)
        {

            var reindexNodesAction = new HealthCheckAction("reindexNodes", Id)
            {
                Name = "Reindex nodes"
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
