using Our.Umbraco.FullTextSearch.Interfaces;
using Our.Umbraco.FullTextSearch.Services.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Persistence;
using Umbraco.Core.Scoping;
using Umbraco.Web;

namespace Our.Umbraco.FullTextSearch.Services
{
    public class CacheService : ICacheService
    {
        private readonly IScopeProvider _scopeProvider;
        private readonly ILogger _logger;
        private readonly IUmbracoContextFactory _umbracoContextFactory;
        private readonly IHtmlService _htmlService;
        private readonly IFullTextSearchConfig _fullTextConfig;
        private readonly IUmbracoComponentRenderer _umbracoComponentRenderer;

        public CacheService(
            IScopeProvider scopeProvider,
            ILogger logger,
            IUmbracoContextFactory umbracoContextFactory,
            IHtmlService htmlService,
            IUmbracoComponentRenderer umbracoComponentRenderer,
            IFullTextSearchConfig config)
        {
            _scopeProvider = scopeProvider;
            _logger = logger;
            _umbracoContextFactory = umbracoContextFactory;
            _htmlService = htmlService;
            _umbracoComponentRenderer = umbracoComponentRenderer;
            _fullTextConfig = config;
        }

        /// <summary>
        /// Adds the content of the node with the id to the FullText Cache, by downloading the content of the nodes urls. One for each culture.
        /// </summary>
        /// <param name="id"></param>
        public void AddToCache(int id)
        {
            using (var cref = _umbracoContextFactory.EnsureUmbracoContext())
            {
                var publishedContent = cref.UmbracoContext.Content.GetById(id);
                if (publishedContent == null || IsDisallowed(publishedContent))
                {
                    // delete from cache if possible, and return
                    DeleteFromCache(id);
                    return;
                }

                foreach (var culture in publishedContent.Cultures)
                {
                    // get content of page, and manipulate for indexing
                    //var url = publishedContent.Url(culture.Value.Culture, UrlMode.Absolute);
                    //_htmlService.GetHtmlByUrl(url, out string fullHtml);
                    if (culture.Value.Culture.IsNullOrWhiteSpace())
                        CultureInfo.CurrentUICulture = new CultureInfo(culture.Value.Culture);

                    cref.UmbracoContext.HttpContext.Items.Add(_fullTextConfig.IndexingActiveKey, "1");

                    var fullHtml = _umbracoComponentRenderer.RenderTemplate(id).ToString();
                    var fullText = _htmlService.GetTextFromHtml(fullHtml);
                    _logger.Info<CacheService>("Updating {nodeId} {culture} {fullText}", id, culture.Value.Culture, fullText);
                    AddToCache(id, culture.Value.Culture, fullText);

                    cref.UmbracoContext.HttpContext.Items.Remove(_fullTextConfig.IndexingActiveKey);
                }
            }
        }

        private bool IsDisallowed(IPublishedContent node)
        {
            if (_fullTextConfig.DisallowedContentTypeAliases.Any() && _fullTextConfig.DisallowedContentTypeAliases.InvariantContains(node.ContentType.Alias)) return true;

            if (_fullTextConfig.DisallowedPropertyAliases.Any())
            {
                foreach (var culture in node.Cultures)
                {
                    foreach (var alias in _fullTextConfig.DisallowedPropertyAliases)
                    {
                        if (node.Value<bool>(alias, culture.Value.Culture)) return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Adds the content to the cache table
        /// </summary>
        /// <param name="id"></param>
        /// <param name="culture"></param>
        /// <param name="text"></param>
        private void AddToCache(int id, string culture, string text)
        {
            using (var scope = _scopeProvider.CreateScope(autoComplete: true))
            {
                var sql = scope.SqlContext.Sql().Select("*").From<CacheItem>().Where<CacheItem>(x => x.NodeId == id && x.Culture == culture);
                var cacheItem = scope.Database.FirstOrDefault<CacheItem>(sql);
                var update = true;

                if (cacheItem == null)
                {
                    cacheItem = new CacheItem
                    {
                        NodeId = id,
                        Culture = culture
                    };
                    update = false;
                }

                cacheItem.Text = text;
                cacheItem.LastUpdated = DateTime.Now;

                if (update)
                {
                    scope.Database.Update(cacheItem);
                }
                else
                {
                    scope.Database.Insert(cacheItem);
                }
            }
        }

        /// <summary>
        /// Deletes the content of the specified node id from the cache table.
        /// </summary>
        /// <param name="id"></param>
        public void DeleteFromCache(int id)
        {
            using (var scope = _scopeProvider.CreateScope(autoComplete: true))
            {
                var sql = scope.SqlContext.Sql().Delete().From<CacheItem>().Where<CacheItem>(x => x.NodeId == id);
                scope.Database.Execute(sql);
            }
        }

        /// <summary>
        /// Gets the cached content for the node specified.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<CacheItem> GetFromCache(int id)
        {
            using (var scope = _scopeProvider.CreateScope(autoComplete: true))
            {
                var sql = scope.SqlContext.Sql().Select("*").From<CacheItem>().Where<CacheItem>(x => x.NodeId == id);
                return scope.Database.Fetch<CacheItem>(sql);
            }
        }
    }
}
