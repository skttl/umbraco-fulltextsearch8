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
using Umbraco.Core.Logging;
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
        public bool ReindexNode(string nodeIds, bool includeDescendants = false)
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
                    _cacheService.AddToCache(id);
                    if (includeDescendants)
                    {
                        var node = Umbraco.Content(id);
                        if (node != null)
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
    }
}
