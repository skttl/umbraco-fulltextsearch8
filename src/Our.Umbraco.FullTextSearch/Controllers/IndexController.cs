using Asp.Versioning;
using Examine;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Our.Umbraco.FullTextSearch.Controllers.Models;
using Our.Umbraco.FullTextSearch.Interfaces;
using Our.Umbraco.FullTextSearch.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Examine;
using Umbraco.Cms.Web.Common;
using Umbraco.Extensions;

namespace Our.Umbraco.FullTextSearch.Controllers;

[ApiVersion("5.0")]
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
    private readonly IStatusService _statusService;
    private readonly UmbracoHelper _umbracoHelper;

    public IndexController(ICacheService cacheService,
        ILogger<IndexController> logger,
        IOptions<FullTextSearchOptions> options,
        IExamineManager examineManager,
        IIndexRebuilder indexRebuilder,
        IPublishedContentValueSetBuilder valueSetBuilder,
        IContentService contentService,
        IStatusService statusService,
        UmbracoHelper umbracoHelper)
    {
        _cacheService = cacheService;
        _options = options.Value;
        _logger = logger;
        _valueSetBuilder = valueSetBuilder;
        _examineManager = examineManager;
        _contentService = contentService;
        _indexRebuilder = indexRebuilder;
        _statusService = statusService;
        _umbracoHelper = umbracoHelper;
    }



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
        if (request.NodeIds == null || !request.NodeIds.Any())
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
                var nodes = _umbracoHelper.Content(request.NodeIds).ToList();
                foreach (var node in nodes)
                {
                    if (request.IncludeDescendants)
                        await _cacheService.AddTreeToCache(node);

                    else await _cacheService.AddToCache(node);
                }

                index.IndexItems(_valueSetBuilder.GetValueSets(_contentService.GetByIds(request.NodeIds).ToArray()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new ReIndexResult(false, ex.Message);
            }
        }

        return new ReIndexResult(true);
    }

    [HttpGet("indexstatus")]
    public IndexStatus GetIndexStatus()
    {
        if (!_options.Enabled)
        {
            _logger.LogDebug("FullTextIndexing is not enabled");
            return null;
        }

        if (!_examineManager.TryGetIndex(Constants.UmbracoIndexes.ExternalIndexName, out IIndex index))
        {
            _logger.LogError(new InvalidOperationException($"No index found by name {Constants.UmbracoIndexes.ExternalIndexName}"), $"No index found by name {Constants.UmbracoIndexes.ExternalIndexName}");
            return null;
        }

        if (_statusService.TryGetIndexableNodes(out ISearchResults indexableNodes)
            && _statusService.TryGetIndexedNodes(out ISearchResults indexedNodes)
            && _statusService.TryGetIncorrectIndexedNodes(out ISearchResults incorrectIndexedNodes)
            && _statusService.TryGetMissingNodes(out ISearchResults missingNodes))
        {

            return new IndexStatus()
            {
                TotalIndexableNodes = (indexableNodes?.TotalItemCount).GetValueOrDefault(),
                TotalIndexedNodes = (indexedNodes?.TotalItemCount).GetValueOrDefault(),
                IncorrectIndexedNodes = (incorrectIndexedNodes?.TotalItemCount).GetValueOrDefault(),
                MissingIndexedNodes = (missingNodes?.TotalItemCount).GetValueOrDefault()
            };
        }

        return null;
    }

    [HttpGet("indexednodes")]
    public IndexedNodeResult GetIndexedNodes(int pageNumber = 1)
    {
        if (!_statusService.TryGetIndexedNodes(out ISearchResults indexedNodes, pageNumber * 100))
        {
            return null;
        }
        var result = new IndexedNodeResult();
        result.PageNumber = pageNumber;
        result.TotalPages = indexedNodes.TotalItemCount / 100;
        result.Items = indexedNodes.Skip((pageNumber - 1) * 100).Select(x => new IndexedNode()
        {
            Id = x.Id,
            Icon = x.GetValues("__Icon").FirstOrDefault(),
            Name = x.GetValues("nodeName").FirstOrDefault(),
            Description = GetBreadcrumb(x.GetValues("__Path").FirstOrDefault())
        }).ToList();

        return result;
    }

    [HttpGet("missingnodes")]
    public IndexedNodeResult GetMissingNodes(int pageNumber = 1)
    {
        if (!_statusService.TryGetMissingNodes(out ISearchResults missingNodes, pageNumber * 100))
        {
            return null;
        }
        var result = new IndexedNodeResult();
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

    [HttpGet("incorrectindexednodes")]
    public IndexedNodeResult GetIncorrectIndexedNodes(int pageNumber = 1)
    {
        if (!_statusService.TryGetIncorrectIndexedNodes(out ISearchResults incorrectIndexedNodes, pageNumber * 100))
        {
            return null;
        }
        var result = new IndexedNodeResult();
        result.PageNumber = pageNumber;
        result.TotalPages = incorrectIndexedNodes.TotalItemCount / 100;
        result.Items = incorrectIndexedNodes.Skip((pageNumber - 1) * 100).Select(x => new IndexedNode()
        {
            Id = x.Id,
            Icon = x.GetValues("__Icon").FirstOrDefault(),
            Name = x.GetValues("nodeName").FirstOrDefault(),
            Description = GetReasonForBeingIncorrect(x)
        }).ToList();

        return result;
    }

    private string GetBreadcrumb(string path)
    {
        var pathParts = path.Split(',').Select(int.Parse).Skip(1);
        var pathNames = pathParts.Select(x => _umbracoHelper.Content(x)?.Name).Where(x => x != null);
        return string.Join(" / ", pathNames);
    }

    private string GetReasonForBeingIncorrect(ISearchResult searchResult)
    {
        var reasons = new List<string>();

        if (_options.DisallowedContentTypeAliases.InvariantContains(searchResult.GetValues("__NodeTypeAlias").FirstOrDefault()))
        {
            reasons.Add(string.Format("content Type ({0}) is disallowed", searchResult.GetValues("__NodeTypeAlias").FirstOrDefault()));
        }

        foreach (var disallowedPropertyAlias in _options.DisallowedPropertyAliases)
        {
            if (searchResult.GetValues(disallowedPropertyAlias)?.FirstOrDefault() == "1")
            {
                reasons.Add(string.Format("disallowed property ({0}) is checked", disallowedPropertyAlias));
            }
        }

        return string.Join(", ", reasons).ToFirstUpper();
    }
}