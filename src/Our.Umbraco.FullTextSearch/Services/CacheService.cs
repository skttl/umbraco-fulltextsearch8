using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Our.Umbraco.FullTextSearch.Interfaces;
using Our.Umbraco.FullTextSearch.Notifications;
using Our.Umbraco.FullTextSearch.Options;
using Our.Umbraco.FullTextSearch.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseModelDefinitions;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Extensions;

namespace Our.Umbraco.FullTextSearch.Services;

public class CacheService(
    IEventMessagesFactory eventMessagesFactory,
    IScopeProvider scopeProvider,
    ILogger<ICacheService> logger,
    IUmbracoContextFactory umbracoContextFactory,
    IHtmlService htmlService,
    IOptions<FullTextSearchOptions> options,
    IPageRenderer pageRenderer)
    : ICacheService
{
    private readonly FullTextSearchOptions _options = options.Value;

    public bool CacheTableExists()
    {
        using var scope = scopeProvider.CreateScope(autoComplete: true);
        return scope.SqlContext.SqlSyntax.DoesTableExist(scope.Database, DefinitionFactory.GetTableDefinition(typeof(CacheItem), scope.SqlContext.SqlSyntax).Name);
    }

    public async Task AddToCache(IPublishedContent publishedContent)
    {
        if (publishedContent == null || IsDisallowed(publishedContent))
        {
            // delete from cache if possible, and return
            if (publishedContent is not null) await DeleteFromCache(publishedContent.Id);
            return;
        }

        await CleanupCultureCache(publishedContent.Id, publishedContent.Cultures.Select(x => x.Value.Culture));

        foreach (var culture in publishedContent.Cultures)
        {
            var fullHtml = await pageRenderer.Render(publishedContent, culture.Value);

            if (fullHtml != null && fullHtml.StartsWith("<!-- Error"))
            {
                logger.LogWarning("FullTextSearch: Error updating nodeId: {nodeId} in culture: {culture} using templateId: {templateId}: {fullHtml}", publishedContent.Id, culture.Value.Culture, publishedContent.TemplateId, fullHtml);
            }

            var fullText = htmlService.GetTextFromHtml(fullHtml);

            logger.LogDebug("Updating nodeId: {nodeId} in culture: {culture} using templateId: {templateId} with content: {fullText}", publishedContent.Id, culture.Value.Culture, publishedContent.TemplateId, fullText);

            await AddToCache(publishedContent.Id, culture.Value.Culture, fullText);

        }

    }

    public async Task AddTreeToCache(IPublishedContent rootNode)
    {
        if (rootNode == null) return;
        await AddToCache(rootNode);

        var children = rootNode.Children();
        foreach (var node in children)
        {
            await AddTreeToCache(node);
        }

    }

    public async Task AddTreeToCache(int rootId)
    {
        using var scope = scopeProvider.CreateScope(autoComplete: true);
        using var cref = umbracoContextFactory.EnsureUmbracoContext();
        await AddTreeToCache(cref.UmbracoContext.Content.GetById(rootId));
    }

    /// <summary>
    /// Adds the content of the node with the id to the FullText Cache, by downloading the content of the nodes urls. One for each culture.
    /// </summary>
    /// <param name="id"></param>
    public async Task AddToCache(int id)
    {
        using var scope = scopeProvider.CreateScope(autoComplete: true);
        using var cref = umbracoContextFactory.EnsureUmbracoContext();
        await AddToCache(cref.UmbracoContext.Content.GetById(id));
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
    private async Task AddToCache(int id, string culture, string text)
    {
        var eventMessages = eventMessagesFactory.Get();
        using var scope = scopeProvider.CreateScope(autoComplete: true);
        var sql = scope.SqlContext.Sql().Select("*").From<CacheItem>().Where<CacheItem>(x => x.NodeId == id && x.Culture == culture);
        var cacheItem = await scope.Database.FirstOrDefaultAsync<CacheItem>(sql).ConfigureAwait(false);
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

        var savingNotification = new CacheSavingNotification(cacheItem, eventMessages);
        if (await scope.Notifications.PublishCancelableAsync(savingNotification).ConfigureAwait(false))
        {
            scope.Complete();
            return;
        }

        if (update)
        {
            await scope.Database.UpdateAsync(cacheItem).ConfigureAwait(false);
        }
        else
        {
            await scope.Database.InsertAsync(cacheItem).ConfigureAwait(false);
        }
        scope.Notifications.Publish(
            new CacheSavedNotification(cacheItem, eventMessages).WithStateFrom(savingNotification));
    }

    /// <summary>
    /// Deletes the content of the specified node id from the cache table.
    /// </summary>
    /// <param name="id"></param>
    public async Task DeleteFromCache(int id)
    {
        using var scope = scopeProvider.CreateScope(autoComplete: true);
        var sql = scope.SqlContext.Sql().Delete().From<CacheItem>().Where<CacheItem>(x => x.NodeId == id);
        await scope.Database.ExecuteAsync(sql).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes the content of the specified node id, in other cultures than specified.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cultures"></param>
    private async Task CleanupCultureCache(int id, IEnumerable<string> cultures)
    {
        using var scope = scopeProvider.CreateScope(autoComplete: true);
        var sql = scope.SqlContext.Sql().Delete().From<CacheItem>().Where<CacheItem>(x => x.NodeId == id && !cultures.Contains(x.Culture));
        await scope.Database.ExecuteAsync(sql).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the cached content for the node specified.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<List<CacheItem>> GetFromCache(int id)
    {
        using var scope = scopeProvider.CreateScope(autoComplete: true);
        var sql = scope.SqlContext.Sql().Select("*").From<CacheItem>().Where<CacheItem>(x => x.NodeId == id);
        return await scope.Database.FetchAsync<CacheItem>(sql).ConfigureAwait(false);
    }
}
