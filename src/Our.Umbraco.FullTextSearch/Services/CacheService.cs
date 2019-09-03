using Our.Umbraco.FullTextSearch.Interfaces;
using Our.Umbraco.FullTextSearch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public CacheService(IScopeProvider scopeProvider, ILogger logger, IUmbracoContextFactory umbracoContextFactory, IHtmlService htmlService)
        {
            _scopeProvider = scopeProvider;
            _logger = logger;
            _umbracoContextFactory = umbracoContextFactory;
            _htmlService = htmlService;
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
                if (publishedContent == null) return;

                foreach (var culture in publishedContent.Cultures)
                {
                    // get content of page, and manipulate for indexing
                    var url = publishedContent.Url(culture.Value.Culture, UrlMode.Absolute);
                    _htmlService.GetHtmlByUrl(url, out string fullHtml);
                    var fullText = _htmlService.GetTextFromHtml(fullHtml);
                    _logger.Info<CacheService>("Updating {nodeId} {culture} {fullText}", id, culture.Value.Culture, fullText);
                    AddToCache(id, culture.Value.Culture, fullText);
                }
            }
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
                scope.Database.Delete(sql);
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

        /// <summary>
        /// Removes the cache task from the table
        /// </summary>
        /// <param name="taskId"></param>
        public void DeleteCacheTask(int taskId)
        {
            using (var scope = _scopeProvider.CreateScope(autoComplete: true))
            {
                scope.Database.Delete<CacheTask>(taskId);
            }
        }

        /// <summary>
        /// Sets a task as started, which means that the task will not be returned by GetCacheTasks again. This is to prevent that a tasks gets run twice.
        /// </summary>
        /// <param name="task"></param>
        public void SetTaskAsStarted(CacheTask task)
        {
            using (var scope = _scopeProvider.CreateScope(autoComplete: true))
            {
                task.Started = true;
                scope.Database.Update(task);
            }

        }

        /// <summary>
        /// Adds a node id as a task for caching the content.
        /// </summary>
        /// <param name="id"></param>
        public void AddCacheTask(int id)
        {
            using (var scope = _scopeProvider.CreateScope(autoComplete: true))
            {
                var sql = scope.SqlContext.Sql().Select("*").From<CacheTask>().Where<CacheTask>(x => x.NodeId == id && x.Started == false);
                var cacheItem = scope.Database.FirstOrDefault<CacheItem>(sql);

                if (cacheItem == null)
                {
                    scope.Database.Insert(new CacheTask() { NodeId = id });
                }
            }
        }

        public List<CacheTask> GetCacheTasks()
        {
            using (var scope = _scopeProvider.CreateScope(autoComplete: true))
            {
                var sql = scope.SqlContext.Sql().Select("*").From<CacheTask>().Where<CacheTask>(x => x.Started == false);
                return scope.Database.Fetch<CacheTask>(sql);
            }
        }
        public List<CacheTask> GetAllCacheTasks()
        {
            using (var scope = _scopeProvider.CreateScope(autoComplete: true))
            {
                var sql = scope.SqlContext.Sql().Select("*").From<CacheTask>();
                return scope.Database.Fetch<CacheTask>(sql);
            }
        }
    }
}
