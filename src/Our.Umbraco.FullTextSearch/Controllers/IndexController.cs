using Asp.Versioning;
using Examine;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Our.Umbraco.FullTextSearch.Controllers.Models;
using Our.Umbraco.FullTextSearch.Interfaces;
using Our.Umbraco.FullTextSearch.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Examine;
using Umbraco.Cms.Web.Common;
using Umbraco.Extensions;

namespace Our.Umbraco.FullTextSearch.Controllers;

[ApiExplorerSettings(GroupName = "fulltextsearch")]
public class IndexController : FullTextSearchControllerBase
{
    private readonly ICacheService _cacheService;
    private readonly FullTextSearchOptions _options;
    private readonly ILogger<IndexController> _logger;
    private readonly IPublishedContentValueSetBuilder _valueSetBuilder;
    private readonly IExamineManager _examineManager;
    private readonly IContentService _contentService;
    private readonly IIndexRebuilder _indexRebuilder;
    private readonly UmbracoHelper _umbracoHelper;

    public IndexController(ICacheService cacheService,
        ILogger<IndexController> logger,
        IOptions<FullTextSearchOptions> options,
        IExamineManager examineManager,
        IIndexRebuilder indexRebuilder,
        IPublishedContentValueSetBuilder valueSetBuilder,
        IContentService contentService,
        UmbracoHelper umbracoHelper)
    {
        _cacheService = cacheService;
        _options = options.Value;
        _logger = logger;
        _valueSetBuilder = valueSetBuilder;
        _examineManager = examineManager;
        _contentService = contentService;
        _indexRebuilder = indexRebuilder;
        _umbracoHelper = umbracoHelper;
    }


    [ApiVersion("5.0")]
    [HttpPost("reindexnodes")]
    [Produces("application/json")]
    public async Task<ReIndexResult> ReIndexNodes([FromBody] ReIndexRequest request)
    {
        if (!_options.Enabled)
        {
            _logger.LogDebug("FullTextIndexing is not enabled");
            return new ReIndexResult(false, "FullTextIndexing is not enabled");
        }

        if (!_examineManager.TryGetIndex(Constants.UmbracoIndexes.ExternalIndexName, out IIndex index))
        {
            _logger.LogError(new InvalidOperationException($"No index found by name {Constants.UmbracoIndexes.ExternalIndexName}"), $"No index found by name {Constants.UmbracoIndexes.ExternalIndexName}");

            return new ReIndexResult(false, $"No index found by name {Constants.UmbracoIndexes.ExternalIndexName}");
        }
        if (request.NodeKey == null)
        {
            try
            {
                var nodes = _umbracoHelper.ContentAtRoot().ToList();

                foreach (var node in nodes)
                {
                    await _cacheService.AddTreeToCache(node);
                }

                index.CreateIndex();
                _indexRebuilder.RebuildIndex(Constants.UmbracoIndexes.ExternalIndexName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new ReIndexResult(false, ex.Message);
            }
        }
        else
        {
            try
            {
                var node = _umbracoHelper.Content(request.NodeKey);
                if (node == null)
                {
                    return new ReIndexResult(false, $"Node {request.NodeKey} not found");
                }

                var nodeIds = new[] { node.Id };
                if (request.IncludeDescendants)
                {
                    await _cacheService.AddTreeToCache(node);
                    nodeIds = nodeIds.Concat(node.Descendants().Select(x => x.Id)).ToArray();
                }
                else
                {
                    await _cacheService.AddToCache(node);
                }
                index.IndexItems(_valueSetBuilder.GetValueSets(_contentService.GetByIds(nodeIds).ToArray()));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new ReIndexResult(false, ex.Message);
            }
        }

        return new ReIndexResult(true);
    }
}