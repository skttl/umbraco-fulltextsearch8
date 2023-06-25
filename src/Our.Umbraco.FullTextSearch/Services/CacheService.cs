using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Our.Umbraco.FullTextSearch.Interfaces;
using Our.Umbraco.FullTextSearch.Options;
using Our.Umbraco.FullTextSearch.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Extensions;

namespace Our.Umbraco.FullTextSearch.Services
{
    public class CacheService : ICacheService
    {
        private readonly IScopeProvider _scopeProvider;
        private readonly ILogger<ICacheService> _logger;
        private readonly IUmbracoContextFactory _umbracoContextFactory;
        private readonly IHtmlService _htmlService;
        private readonly FullTextSearchOptions _options;
        private readonly IPageRenderer _pageRenderer;

        public CacheService(
            IScopeProvider scopeProvider,
            ILogger<ICacheService> logger,
            IUmbracoContextFactory umbracoContextFactory,
            IHtmlService htmlService,
            IOptions<FullTextSearchOptions> options,
            IPageRenderer pageRenderer
            )
        {
            _scopeProvider = scopeProvider;
            _logger = logger;
            _umbracoContextFactory = umbracoContextFactory;
            _htmlService = htmlService;
            _options = options.Value;
            _pageRenderer = pageRenderer;
        }

        public async Task AddToCache(IPublishedContent publishedContent)
        {
            if (publishedContent == null || IsDisallowed(publishedContent))
            {
                // delete from cache if possible, and return
                if (publishedContent is not null) DeleteFromCache(publishedContent.Id);
                return;
            }

            await CleanupCultureCache(publishedContent.Id, publishedContent.Cultures.Select(x => x.Value.Culture));

            foreach (var culture in publishedContent.Cultures)
            {
                var fullHtml = await _pageRenderer.Render(publishedContent, culture.Value);

                var fullText = _htmlService.GetTextFromHtml(fullHtml);

                _logger.LogDebug("Updating nodeId: {nodeId} in culture: {culture} using templateId: {templateId} with content: {fullText}", publishedContent.Id, culture.Value.Culture, publishedContent.TemplateId, fullText);

                AddToCache(publishedContent.Id, culture.Value.Culture, fullText);

            }

            return;
        }

        public async Task AddTreeToCache(IPublishedContent rootNode)
        {
            if (rootNode == null) return; ;
            await AddToCache(rootNode);
            rootNode.Children.ToList().ForEach(node => AddTreeToCache(node));

            return;
        }

        public async Task AddTreeToCache(int rootId)
        {
            using (var scope = _scopeProvider.CreateScope(autoComplete: true))
            {
                using (var cref = _umbracoContextFactory.EnsureUmbracoContext())
                {
                    await AddTreeToCache(cref.UmbracoContext.Content.GetById(rootId));
                }
            }

            return;
        }

        /// <summary>
        /// Adds the content of the node with the id to the FullText Cache, by downloading the content of the nodes urls. One for each culture.
        /// </summary>
        /// <param name="id"></param>
        public async Task AddToCache(int id)
        {
            using (var scope = _scopeProvider.CreateScope(autoComplete: true))
            {
                using (var cref = _umbracoContextFactory.EnsureUmbracoContext())
                {
                    await AddToCache(cref.UmbracoContext.Content.GetById(id));
                }
            }
        }

        private bool IsDisallowed(IPublishedContent node)
        {
            if (!node.TemplateId.HasValue || node.TemplateId.Value <= 0) return true;

            if (_options.DisallowedContentTypeAliases.Any() && _options.DisallowedContentTypeAliases.InvariantContains(node.ContentType.Alias)) return true;

            if (_options.DisallowedPropertyAliases.Any())
            {
                foreach (var culture in node.Cultures)
                {
                    foreach (var alias in _options.DisallowedPropertyAliases)
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
        private async void AddToCache(int id, string culture, string text)
        {
            using (var scope = _scopeProvider.CreateScope(autoComplete: true))
            {
                var sql = scope.SqlContext.Sql().Select("*").From<CacheItem>().Where<CacheItem>(x => x.NodeId == id && x.Culture == culture);
                var cacheItem = await scope.Database.FirstOrDefaultAsync<CacheItem>(sql);
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
                    await scope.Database.UpdateAsync(cacheItem);
                }
                else
                {
                    await scope.Database.InsertAsync(cacheItem);
                }
            }
        }

        /// <summary>
        /// Deletes the content of the specified node id from the cache table.
        /// </summary>
        /// <param name="id"></param>
        public async Task DeleteFromCache(int id)
        {
            using (var scope = _scopeProvider.CreateScope(autoComplete: true))
            {
                var sql = scope.SqlContext.Sql().Delete().From<CacheItem>().Where<CacheItem>(x => x.NodeId == id);
                await scope.Database.ExecuteAsync(sql);
            }
        }

        /// <summary>
        /// Deletes the content of the specified node id, in other cultures than specified.
        /// </summary>
        /// <param name="id"></param>
        public async Task CleanupCultureCache(int id, IEnumerable<string> cultures)
        {
            using (var scope = _scopeProvider.CreateScope(autoComplete: true))
            {
                var sql = scope.SqlContext.Sql().Delete().From<CacheItem>().Where<CacheItem>(x => x.NodeId == id && !cultures.Contains(x.Culture));
                await scope.Database.ExecuteAsync(sql);
            }
        }

        /// <summary>
        /// Gets the cached content for the node specified.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<CacheItem>> GetFromCache(int id)
        {
            using (var scope = _scopeProvider.CreateScope(autoComplete: true))
            {
                var sql = scope.SqlContext.Sql().Select("*").From<CacheItem>().Where<CacheItem>(x => x.NodeId == id);
                return await scope.Database.FetchAsync<CacheItem>(sql);
            }
        }
    }
}
