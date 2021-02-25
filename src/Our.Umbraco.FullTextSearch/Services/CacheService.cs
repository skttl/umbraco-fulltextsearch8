using System;
using System.Collections.Generic;
using System.Linq;
using Our.Umbraco.FullTextSearch.Interfaces;
using Our.Umbraco.FullTextSearch.Services.Models;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Persistence;
using Umbraco.Core.Scoping;
using Umbraco.Core.Services;
using Umbraco.Web;

namespace Our.Umbraco.FullTextSearch.Services
{
    public class CacheService : ICacheService
    {
        private readonly IScopeProvider _scopeProvider;
        private readonly ILogger _logger;
        private readonly IUmbracoContextFactory _umbracoContextFactory;
        private readonly IFileService _fileService;
        private readonly IHtmlService _htmlService;
        private readonly IFullTextSearchConfig _fullTextConfig;
        private readonly IUmbracoComponentRenderer _umbracoComponentRenderer;
        private readonly IVariationContextAccessor _variationContextAccessor;

        public CacheService(
            IScopeProvider scopeProvider,
            ILogger logger,
            IUmbracoContextFactory umbracoContextFactory,
            IFileService fileService,
            IHtmlService htmlService,
            IUmbracoComponentRenderer umbracoComponentRenderer,
            IFullTextSearchConfig config,
            IVariationContextAccessor variationContextAccessor)
        {
            _scopeProvider = scopeProvider;
            _logger = logger;
            _umbracoContextFactory = umbracoContextFactory;
            _fileService = fileService;
            _htmlService = htmlService;
            _umbracoComponentRenderer = umbracoComponentRenderer;
            _fullTextConfig = config;
            _variationContextAccessor = variationContextAccessor;
        }

        /// <summary>
        /// Adds the content of the node with the id to the FullText Cache, by downloading the content of the nodes urls. One for each culture.
        /// </summary>
        /// <param name="id"></param>
        public void AddToCache(int id)
        {
            using (var scope = _scopeProvider.CreateScope(autoComplete: true))
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

                    CleanupCultureCache(id, publishedContent.Cultures.Select(x => x.Value.Culture));

                    foreach (var culture in publishedContent.Cultures)
                    {
                        // get content of page, and manipulate for indexing
                        //var url = publishedContent.Url(culture.Value.Culture, UrlMode.Absolute);
                        //_htmlService.GetHtmlByUrl(url, out string fullHtml);
                        if (!culture.Value.Culture.IsNullOrWhiteSpace())
                            _variationContextAccessor.VariationContext = new VariationContext(culture.Value.Culture);

                        cref.UmbracoContext.HttpContext.Items.Add(_fullTextConfig.IndexingActiveKey, "1");

                        var templateId = GetRenderingTemplateId();
                        var fullHtml = _umbracoComponentRenderer.RenderTemplate(id, templateId).ToString();
                        var fullText = _htmlService.GetTextFromHtml(fullHtml);
                        _logger.Debug<CacheService>("Updating nodeId: {nodeId} in culture: {culture} using templateId: {templateId} with content: {fullText}", id, culture.Value.Culture, templateId, fullText);
                        AddToCache(id, culture.Value.Culture, fullText);

                        cref.UmbracoContext.HttpContext.Items.Remove(_fullTextConfig.IndexingActiveKey);
                    }
                }
            }
        }

        private int? GetRenderingTemplateId()
        {
            var template = _fileService.GetTemplate("OurUmbracoFullTextRendering");
            if (template != null) return template.Id;

            try
            {
                template = _fileService.CreateTemplateWithIdentity(
                    "FullTextSearch Rendering Template",
                    "OurUmbracoFullTextRendering",
                    RenderingTemplateContent);

                _fileService.SaveTemplate(template);
                return template.Id;
            }
            catch (Exception ex)
            {
                _logger.Error<CacheService>(ex, "Failed creating rendering template");
            }

            return null;
        }

        private string RenderingTemplateContent =>
              "@inherits Umbraco.Web.Mvc.UmbracoViewPage"
            + Environment.NewLine + "@Umbraco.RenderTemplate(Model.Id)"
            + Environment.NewLine + "@*"
            + Environment.NewLine + "This template has been created for the sole purpose of stopping rendering errors"
            + Environment.NewLine + "from bubbling into the backoffice, when caching content for FullTextSearch."
            + Environment.NewLine + ""
            + Environment.NewLine + "For some reason, when doing _umbracoComponentRenderer.RenderTemplate() as a part"
            + Environment.NewLine + "of an event in Umbraco, YSODs bubble through to the backoffice, with no error"
            + Environment.NewLine + "message, leaving the user with no clue about what happened. The user can be led"
            + Environment.NewLine + "believe that the Save&Publish failed - but it didn't. The node just couldn't be"
            + Environment.NewLine + "rendered for some reason, most likely a bug in the nodes template."
            + Environment.NewLine + ""
            + Environment.NewLine + "Please keep this template as it is - if you delete it, it will get re-created the"
            + Environment.NewLine + "next time something needs to be cached for indexing with Full Text Search."
            + Environment.NewLine + ""
            + Environment.NewLine + "Thank you for your understanding. And remember; if you like FullTextSearch,"
            + Environment.NewLine + "you can always buy me a virtual coffee at https://ko-fi.com/skttl"
            + Environment.NewLine + "*@";

        private bool IsDisallowed(IPublishedContent node)
        {
            if (!node.TemplateId.HasValue || node.TemplateId.Value <= 0) return true;

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
        /// Deletes the content of the specified node id, in other cultures than specified.
        /// </summary>
        /// <param name="id"></param>
        public void CleanupCultureCache(int id, IEnumerable<string> cultures)
        {
            using (var scope = _scopeProvider.CreateScope(autoComplete: true))
            {
                var sql = scope.SqlContext.Sql().Delete().From<CacheItem>().Where<CacheItem>(x => x.NodeId == id && !cultures.Contains(x.Culture));
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
