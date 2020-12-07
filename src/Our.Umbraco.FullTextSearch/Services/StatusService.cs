using Examine;
using Our.Umbraco.FullTextSearch.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.Core.Logging;
using Umbraco.Core.Scoping;
using Umbraco.Web;

namespace Our.Umbraco.FullTextSearch.Services
{
    public class StatusService : IStatusService
    {
        private readonly IFullTextSearchConfig _fullTextConfig;
        private readonly IScopeProvider _scopeProvider;
        private readonly IUmbracoContextFactory _umbracoContextFactory;
        private readonly ILogger _logger;
        private readonly IExamineManager _examineManager;

        private string _allIndexableNodesQuery => "__IndexType:content AND __Published:y AND -(templateID:0)";
        private string _allIndexedNodesQuery => _allIndexableNodesQuery + $" AND {_fullTextConfig.FullTextPathField}:\"-1\"";


        public StatusService(
            IFullTextSearchConfig fullTextConfig,
            IScopeProvider scopeProvider,
            IUmbracoContextFactory umbracoContextFactory,
            ILogger logger,
            IExamineManager examineManager)
        {
            _fullTextConfig = fullTextConfig;
            _scopeProvider = scopeProvider;
            _umbracoContextFactory = umbracoContextFactory;
            _logger = logger;
            _examineManager = examineManager;
        }

        private bool TryGetSearcher(out ISearcher searcher)
        {
            searcher = null;
            if (!_fullTextConfig.Enabled)
            {
                _logger.Debug<StatusService>("FullTextIndexing is not enabled");
                return false;
            }

            if (!_examineManager.TryGetIndex("ExternalIndex", out IIndex index))
            {
                _logger.Error<StatusService>(new InvalidOperationException("No index found by name ExternalIndex"));
                return false;
            }
            else
            {
                searcher = index.GetSearcher();
                return true;
            }
        }

        public bool TryGetIndexableNodes(out ISearchResults results, int maxResults = int.MaxValue)
        {
            if (!TryGetSearcher(out ISearcher searcher))
            {
                results = null;
                return false;
            }

            var indexableQuery = new StringBuilder(_allIndexableNodesQuery);
            var disallowed = new List<string>();
            disallowed.AddRange(_fullTextConfig.DisallowedContentTypeAliases.Select(x => $"__NodeTypeAlias:\"{x}\""));
            disallowed.AddRange(_fullTextConfig.DisallowedPropertyAliases.Select(x => $"{x}:1"));

            if (disallowed.Any()) indexableQuery.Append($" AND -({string.Join(" OR ", disallowed)})");

            _logger.Debug<StatusService>("GetIndexableNodes using query {query}", indexableQuery.ToString());

            results = searcher.CreateQuery().NativeQuery(indexableQuery.ToString()).Execute(maxResults);
            return true;
        }

        public bool TryGetIndexedNodes(out ISearchResults results, int maxResults = int.MaxValue)
        {
            if (!TryGetSearcher(out ISearcher searcher))
            {
                results = null;
                return false;
            }
            var indexedQuery = new StringBuilder(_allIndexedNodesQuery);

            _logger.Debug<StatusService>("GetIndexedNodes using query {query}", indexedQuery.ToString());

            results = searcher.CreateQuery().NativeQuery(indexedQuery.ToString()).Execute(maxResults);
            return true;
        }

        public bool TryGetIncorrectIndexedNodes(out ISearchResults results, int maxResults = int.MaxValue)
        {
            if (!TryGetSearcher(out ISearcher searcher))
            {
                results = null;
                return false;
            }
            if (!_fullTextConfig.DisallowedContentTypeAliases.Any() && !_fullTextConfig.DisallowedPropertyAliases.Any())
            {
                results = null;
                return true;
            }

            var incorrectQuery = new StringBuilder(_allIndexedNodesQuery);
            var disallowed = new List<string>();
            disallowed.AddRange(_fullTextConfig.DisallowedContentTypeAliases.Select(x => $"__NodeTypeAlias:\"{x}\""));
            disallowed.AddRange(_fullTextConfig.DisallowedPropertyAliases.Select(x => $"{x}:1"));
            if (disallowed.Any()) incorrectQuery.Append($" AND ({string.Join(" OR ", disallowed)})");

            _logger.Debug<StatusService>("GetIncorrectIndexedNodes using query {query}", incorrectQuery.ToString());

            results = searcher.CreateQuery().NativeQuery(incorrectQuery.ToString()).Execute(maxResults);
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
            missingQuery.Append($" AND -({_fullTextConfig.FullTextPathField}:\"-1\")");

            var disallowed = new List<string>();
            disallowed.AddRange(_fullTextConfig.DisallowedContentTypeAliases.Select(x => $"__NodeTypeAlias:\"{x}\""));
            disallowed.AddRange(_fullTextConfig.DisallowedPropertyAliases.Select(x => $"{x}:1"));
            if (disallowed.Any()) missingQuery.Append($" AND -({string.Join(" OR ", disallowed)})");

            _logger.Debug<StatusService>("GetMissingNodes using query {query}", missingQuery.ToString());

            results = searcher.CreateQuery().NativeQuery(missingQuery.ToString()).Execute(maxResults);
            return true;
        }
    }
}
