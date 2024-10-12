using Examine;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Our.Umbraco.FullTextSearch.Interfaces;
using Our.Umbraco.FullTextSearch.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Web;

namespace Our.Umbraco.FullTextSearch.Services
{
    public class StatusService : IStatusService
    {
        private readonly FullTextSearchOptions _options;
        private readonly ILogger<IStatusService> _logger;
        private readonly IExamineManager _examineManager;

        private string _allIndexableNodesQuery => "__IndexType:content AND __Published:y AND -(templateID:0)";
        private string _allIndexedNodesQuery => _allIndexableNodesQuery + $" AND {_options.FullTextPathField}:\"-1\"";


        public StatusService(
            IOptions<FullTextSearchOptions> options,
            ILogger<IStatusService> logger,
            IExamineManager examineManager)
        {
            _options = options.Value;
            _logger = logger;
            _examineManager = examineManager;
        }

        private bool TryGetSearcher(out ISearcher searcher)
        {
            searcher = null;
            if (!_options.Enabled)
            {
                _logger.LogDebug("FullTextIndexing is not enabled");
                return false;
            }

            if (!_examineManager.TryGetIndex(Constants.UmbracoIndexes.ExternalIndexName, out IIndex index))
            {
                _logger.LogError(new InvalidOperationException($"No index found by name {Constants.UmbracoIndexes.ExternalIndexName}"), $"No index found by name {Constants.UmbracoIndexes.ExternalIndexName}");
                return false;
            }
            else
            {
                searcher = index.Searcher;
                return true;
            }
        }

        public bool TryGetIncorrectIndexedNodes(out ISearchResults results, int maxResults = int.MaxValue)
        {
            if (!TryGetSearcher(out ISearcher searcher))
            {
                results = null;
                return false;
            }
            if (!_options.DisallowedContentTypeAliases.Any() && !_options.DisallowedPropertyAliases.Any())
            {
                results = null;
                return true;
            }

            var incorrectQuery = new StringBuilder(_allIndexedNodesQuery);
            var disallowed = new List<string>();
            disallowed.AddRange(_options.DisallowedContentTypeAliases.Select(x => $"__NodeTypeAlias:\"{x}\""));
            disallowed.AddRange(_options.DisallowedPropertyAliases.Select(x => $"{x}:1"));
            if (disallowed.Any()) incorrectQuery.Append($" AND ({string.Join(" OR ", disallowed)})");

            _logger.LogDebug("GetIncorrectIndexedNodes using query {query}", incorrectQuery.ToString());

            results = searcher.CreateQuery().NativeQuery(incorrectQuery.ToString()).Execute(new Examine.Search.QueryOptions(0, maxResults));
            return true;
        }

        public bool TryGetMissingNodes(out ISearchResults results, int maxResults = int.MaxValue)
        {
            if (!TryGetSearcher(out ISearcher searcher))
            {
                results = null;
                return false;
            }

            var missingQuery = new StringBuilder(_allIndexableNodesQuery);
            missingQuery.Append($" AND -({_options.FullTextPathField}:\"-1\")");

            var disallowed = new List<string>();
            disallowed.AddRange(_options.DisallowedContentTypeAliases.Select(x => $"__NodeTypeAlias:\"{x}\""));
            disallowed.AddRange(_options.DisallowedPropertyAliases.Select(x => $"{x}:1"));
            if (disallowed.Any()) missingQuery.Append($" AND -({string.Join(" OR ", disallowed)})");

            _logger.LogDebug("GetMissingNodes using query {query}", missingQuery.ToString());

            results = searcher.CreateQuery().NativeQuery(missingQuery.ToString()).Execute(new Examine.Search.QueryOptions(0, maxResults));
            return true;
        }
    }
}
