using Examine;
using Microsoft.SqlServer.Server;
using Our.Umbraco.FullTextSearch.Interfaces;
using Our.Umbraco.FullTextSearch.Models;
using Our.Umbraco.FullTextSearch.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Services;
using Umbraco.Examine;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace Our.Umbraco.FullTextSearch.Controllers
{
    [PluginController("FullTextSearch")]
    public class IndexController : UmbracoAuthorizedApiController
    {
        private readonly ICacheService _cacheService;
        private readonly FullTextSearchConfig _fullTextConfig;
        private readonly ILogger _logger;
        private readonly IPublishedContentValueSetBuilder _valueSetBuilder;
        private readonly IExamineManager _examineManager;
        private readonly IContentService _contentService;
        private readonly IndexRebuilder _indexRebuilder;

        private string _allIndexableNodesQuery => "__IndexType:content AND __Published:y AND -(templateID:0)";
        private string _allIndexedNodesQuery => _allIndexableNodesQuery + $" AND {_fullTextConfig.FullTextPathField}:\"-1\"";

        public IndexController(ICacheService cacheService,
            ILogger logger,
            FullTextSearchConfig fullTextConfig,
            IExamineManager examineManager,
            IndexRebuilder indexRebuilder,
            IPublishedContentValueSetBuilder valueSetBuilder,
            IContentService contentService)
        {
            _cacheService = cacheService;
            _fullTextConfig = fullTextConfig;
            _logger = logger;
            _valueSetBuilder = valueSetBuilder;
            _examineManager = examineManager;
            _contentService = contentService;
            _indexRebuilder = indexRebuilder;
        }

        [HttpPost]
        public bool ReIndexNodes(string nodeIds, bool includeDescendants = false)
        {
            if (!_fullTextConfig.Enabled)
            {
                _logger.Debug<IndexController>("FullTextIndexing is not enabled");
                return false;
            }

            if (!_examineManager.TryGetIndex("ExternalIndex", out IIndex index))
            {
                _logger.Error<IndexController>(new InvalidOperationException("No index found by name ExternalIndex"));
                return false;
            }
            if (nodeIds == "*")
            {
                var contentAtRoot = Umbraco.ContentAtRoot().ToList();
                foreach (var content in contentAtRoot)
                {
                    _cacheService.AddToCache(content.Id);

                    var descendants = content.Descendants().ToList();
                    foreach (var descendant in descendants)
                    {
                        _cacheService.AddToCache(descendant.Id);
                    }
                }
                index.CreateIndex();
                _indexRebuilder.RebuildIndex("ExternalIndex");
            }
            else
            {
                var ids = nodeIds.Split(',').Select(x => int.Parse(x));

                foreach (var id in ids)
                {
                    var node = Umbraco.Content(id);
                    if (node != null)
                    {
                        _cacheService.AddToCache(id);

                        if (includeDescendants)
                        {
                            var descendants = node.Descendants().ToList();
                            foreach (var descendant in descendants)
                            {
                                _cacheService.AddToCache(descendant.Id);
                            }
                        }
                    }
                }
                index.IndexItems(_valueSetBuilder.GetValueSets(_contentService.GetByIds(ids).ToArray()));
            }

            return true;
        }

        [HttpGet]
        public IndexStatus GetIndexStatus()
        {
            if (!_fullTextConfig.Enabled)
            {
                _logger.Debug<IndexController>("FullTextIndexing is not enabled");
                return null;
            }

            if (!_examineManager.TryGetIndex("ExternalIndex", out IIndex index))
            {
                _logger.Error<IndexController>(new InvalidOperationException("No index found by name ExternalIndex"));
                return null;
            }

            var status = new IndexStatus();

            var searcher = index.GetSearcher();

            // get all indexable nodes
            status.TotalIndexableNodes = GetIndexableNodes(searcher).TotalItemCount;

            // get all indexed nodes
            status.TotalIndexedNodes = GetIndexedNodes(searcher).TotalItemCount;

            // incorrect indexed nodes
            if (GetIncorrectIndexedNodes(searcher) is ISearchResults incorrectIndexedNodesResult && incorrectIndexedNodesResult != null)
                status.IncorrectIndexedNodes = incorrectIndexedNodesResult.TotalItemCount;

            // missing indexed nodes
            status.MissingIndexedNodes = GetMissingNodes(searcher).TotalItemCount;


            return status;
        }

        [HttpGet]
        public IndexedNodeResult GetIndexedNodes(int pageNumber = 1)
        {
            if (!_fullTextConfig.Enabled)
            {
                _logger.Debug<IndexController>("FullTextIndexing is not enabled");
                return null;
            }

            if (!_examineManager.TryGetIndex("ExternalIndex", out IIndex index))
            {
                _logger.Error<IndexController>(new InvalidOperationException("No index found by name ExternalIndex"));
                return null;
            }

            var result = new IndexedNodeResult();
            var searcher = index.GetSearcher();

            var missingNodes = GetIndexedNodes(searcher, pageNumber * 100);
            result.PageNumber = pageNumber;
            result.TotalPages = missingNodes.TotalItemCount / 100;
            result.Items = missingNodes.Skip((pageNumber - 1) * 100).Select(x => new IndexedNode()
            {
                Id = x.Id,
                Icon = x.GetValues("__Icon").FirstOrDefault(),
                Name = x.GetValues("nodeName").FirstOrDefault(),
                Description = GetBreadcrumb(x.GetValues("__Path").FirstOrDefault())
            }).ToList();

            return result;
        }

        [HttpGet]
        public IndexedNodeResult GetMissingNodes(int pageNumber = 1)
        {
            if (!_fullTextConfig.Enabled)
            {
                _logger.Debug<IndexController>("FullTextIndexing is not enabled");
                return null;
            }

            if (!_examineManager.TryGetIndex("ExternalIndex", out IIndex index))
            {
                _logger.Error<IndexController>(new InvalidOperationException("No index found by name ExternalIndex"));
                return null;
            }

            var result = new IndexedNodeResult();
            var searcher = index.GetSearcher();

            var missingNodes = GetMissingNodes(searcher, pageNumber * 100);
            result.PageNumber = pageNumber;
            result.TotalPages = missingNodes.TotalItemCount / 100;
            result.Items = missingNodes.Skip((pageNumber - 1) * 100).Select(x => new IndexedNode()
            {
                Id = x.Id,
                Icon = x.GetValues("__Icon").FirstOrDefault(),
                Name = x.GetValues("nodeName").FirstOrDefault(),
                Description = GetBreadcrumb(x.GetValues("__Path").FirstOrDefault())
            }).ToList();

            return result;
        }

        [HttpGet]
        public IndexedNodeResult GetIncorrectIndexedNodes(int pageNumber = 1)
        {
            if (!_fullTextConfig.Enabled)
            {
                _logger.Debug<IndexController>("FullTextIndexing is not enabled");
                return null;
            }

            if (!_examineManager.TryGetIndex("ExternalIndex", out IIndex index))
            {
                _logger.Error<IndexController>(new InvalidOperationException("No index found by name ExternalIndex"));
                return null;
            }

            var result = new IndexedNodeResult();
            var searcher = index.GetSearcher();

            var missingNodes = GetIncorrectIndexedNodes(searcher, pageNumber * 100);
            result.PageNumber = pageNumber;
            result.TotalPages = missingNodes.TotalItemCount / 100;
            result.Items = missingNodes.Skip((pageNumber - 1) * 100).Select(x => new IndexedNode()
            {
                Id = x.Id,
                Icon = x.GetValues("__Icon").FirstOrDefault(),
                Name = x.GetValues("nodeName").FirstOrDefault(),
                Description = GetReasonForBeingIncorrect(x)
            }).ToList();

            return result;
        }

        private string GetReasonForBeingIncorrect(ISearchResult searchResult)
        {
            var reasons = new List<string>();

            if (_fullTextConfig.DisallowedContentTypeAliases.Contains(searchResult.GetValues("__NodeTypeAlias").FirstOrDefault()))
            {
                reasons.Add(string.Format("content Type ({0}) is disallowed", searchResult.GetValues("__NodeTypeAlias").FirstOrDefault()));
            }

            foreach (var disallowedPropertyAlias in _fullTextConfig.DisallowedPropertyAliases)
            {
                if (searchResult.GetValues(disallowedPropertyAlias)?.FirstOrDefault() == "1")
                {
                    reasons.Add(string.Format("disallowed property ({0}) is checked", disallowedPropertyAlias));
                }
            }

            return string.Join(", ", reasons).ToFirstUpper();
        }

        private string GetBreadcrumb(string path)
        {
            var pathParts = path.Split(',').Select(int.Parse).Skip(1);
            var pathNames = pathParts.Select(x => Umbraco.Content(x)?.Name).Where(x => x != null);
            return string.Join(" / ", pathNames);
        }

        private ISearchResults GetIndexableNodes(ISearcher searcher, int maxResults = int.MaxValue)
        {
            var indexableQuery = new StringBuilder(_allIndexableNodesQuery);
            if (_fullTextConfig.DisallowedContentTypeAliases.Any())
            {
                var disallowedContentTypeAliasGroup = string.Join(" OR ", _fullTextConfig.DisallowedContentTypeAliases.Select(x => $"__NodeTypeAlias:{x}"));
                indexableQuery.Append($" AND -({disallowedContentTypeAliasGroup})");
            }

            if (_fullTextConfig.DisallowedPropertyAliases.Any())
            {
                var disallowedPropertyAliasGroup = string.Join(" OR ", _fullTextConfig.DisallowedPropertyAliases.Select(x => $"{x}:1"));
                indexableQuery.Append($" AND -({disallowedPropertyAliasGroup})");
            }


            _logger.Debug<IndexController>("GetIndexableNodes using query {query}", indexableQuery.ToString());

            return searcher.CreateQuery().NativeQuery(indexableQuery.ToString()).Execute(maxResults);
        }

        private ISearchResults GetIndexedNodes(ISearcher searcher, int maxResults = int.MaxValue)
        {
            var indexedQuery = new StringBuilder(_allIndexedNodesQuery);

            _logger.Debug<IndexController>("GetIndexedNodes using query {query}", indexedQuery.ToString());

            return searcher.CreateQuery().NativeQuery(indexedQuery.ToString()).Execute(maxResults);
        }

        private ISearchResults GetIncorrectIndexedNodes(ISearcher searcher, int maxResults = int.MaxValue)
        {
            if (!_fullTextConfig.DisallowedContentTypeAliases.Any() && !_fullTextConfig.DisallowedPropertyAliases.Any()) return null;

            var incorrectQuery = new StringBuilder(_allIndexedNodesQuery);
            if (_fullTextConfig.DisallowedContentTypeAliases.Any())
            {
                var disallowedContentTypeAliasGroup = string.Join(" OR ", _fullTextConfig.DisallowedContentTypeAliases.Select(x => $"__NodeTypeAlias:{x}"));
                incorrectQuery.Append($" AND ({disallowedContentTypeAliasGroup})");
            }

            if (_fullTextConfig.DisallowedPropertyAliases.Any())
            {
                var disallowedPropertyAliasGroup = string.Join(" OR ", _fullTextConfig.DisallowedPropertyAliases.Select(x => $"{x}:1"));
                incorrectQuery.Append($" AND ({disallowedPropertyAliasGroup})");
            }

            _logger.Debug<IndexController>("GetIncorrectIndexedNodes using query {query}", incorrectQuery.ToString());

            return searcher.CreateQuery().NativeQuery(incorrectQuery.ToString()).Execute(maxResults);
        }

        private ISearchResults GetMissingNodes(ISearcher searcher, int maxResults = int.MaxValue)
        {
            var missingQuery = new StringBuilder(_allIndexableNodesQuery);
            missingQuery.Append($" AND -({_fullTextConfig.FullTextPathField}:\"-1\")");
            if (_fullTextConfig.DisallowedContentTypeAliases.Any() || _fullTextConfig.DisallowedPropertyAliases.Any())
            {
                if (_fullTextConfig.DisallowedContentTypeAliases.Any())
                {
                    var disallowedContentTypeAliasGroup = string.Join(" OR ", _fullTextConfig.DisallowedContentTypeAliases.Select(x => $"__NodeTypeAlias:{x}"));
                    missingQuery.Append($" AND -({disallowedContentTypeAliasGroup})");
                }

                if (_fullTextConfig.DisallowedPropertyAliases.Any())
                {
                    var disallowedPropertyAliasGroup = string.Join(" OR ", _fullTextConfig.DisallowedPropertyAliases.Select(x => $"{x}:1"));
                    missingQuery.Append($" AND -({disallowedPropertyAliasGroup})");
                }
            }

            _logger.Debug<IndexController>("GetMissingNodes using query {query}", missingQuery.ToString());

            return searcher.CreateQuery().NativeQuery(missingQuery.ToString()).Execute(maxResults);
        }
    }
}
