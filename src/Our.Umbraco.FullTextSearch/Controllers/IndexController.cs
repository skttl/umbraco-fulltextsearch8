using Examine;
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
        public bool RebuildCache(string nodeIds, bool includeDescendants = false)
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

            var query = new StringBuilder();

            // get all indexable nodes
            query.Append($"__IndexType:content AND __Published:y AND -(templateID:0)");
            var indexableNodesResult = searcher.CreateQuery().NativeQuery(query.ToString()).Execute();
            status.TotalIndexableNodes = indexableNodesResult.TotalItemCount;

            // get all indexed nodes
            var allQuery = new StringBuilder(query.ToString());
            allQuery.Append($" AND {_fullTextConfig.FullTextPathField}:\"-1\"");
            var indexedNodesResult = searcher.CreateQuery().NativeQuery(allQuery.ToString()).Execute();
            status.TotalIndexedNodes = indexedNodesResult.TotalItemCount;

            // incorrect indexed nodes
            if (_fullTextConfig.DisallowedContentTypeAliases.Any() || _fullTextConfig.DisallowedPropertyAliases.Any())
            {
                var incorrectQuery = new StringBuilder(allQuery.ToString());
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
                var incorrectIndexedNodesResult = searcher.CreateQuery().NativeQuery(incorrectQuery.ToString()).Execute();
                status.IncorrectIndexedNodes = incorrectIndexedNodesResult.TotalItemCount;
            }

            // missing indexed nodes
            var missingQuery = new StringBuilder(query.ToString());
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

            var missingResult = searcher.CreateQuery().NativeQuery(missingQuery.ToString()).Execute();
            status.MissingIndexedNodes = missingResult.TotalItemCount;


            return status;
        }
    }
}
