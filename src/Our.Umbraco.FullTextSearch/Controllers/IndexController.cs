using Examine;
using Our.Umbraco.FullTextSearch.Controllers.Models;
using Our.Umbraco.FullTextSearch.Interfaces;
using Our.Umbraco.FullTextSearch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Services;
using Umbraco.Examine;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;
using UPC = Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.FullTextSearch.Controllers
{
    [PluginController("FullTextSearch")]
    public class IndexController : UmbracoAuthorizedApiController
    {
        private readonly ICacheService _cacheService;
        private readonly IFullTextSearchConfig _fullTextConfig;
        private readonly ILogger _logger;
        private readonly IPublishedContentValueSetBuilder _valueSetBuilder;
        private readonly IExamineManager _examineManager;
        private readonly IContentService _contentService;
        private readonly IndexRebuilder _indexRebuilder;
        private readonly ILocalizationService _localizationService;
        private readonly ISearchService _searchService;
        private readonly IStatusService _statusService;

        public IndexController(ICacheService cacheService,
            ILogger logger,
            IFullTextSearchConfig fullTextConfig,
            IExamineManager examineManager,
            IndexRebuilder indexRebuilder,
            IPublishedContentValueSetBuilder valueSetBuilder,
            IContentService contentService,
            ILocalizationService localizationService,
            ISearchService searchService,
            IStatusService statusService)
        {
            _cacheService = cacheService;
            _fullTextConfig = fullTextConfig;
            _logger = logger;
            _valueSetBuilder = valueSetBuilder;
            _examineManager = examineManager;
            _contentService = contentService;
            _indexRebuilder = indexRebuilder;
            _localizationService = localizationService;
            _searchService = searchService;
            _statusService = statusService;
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
                    AddToCacheDescendants(content);
                }
                index.CreateIndex();
                _indexRebuilder.RebuildIndex("ExternalIndex");
            }
            else
            {
                var ids = nodeIds.Split(',').Select(x => int.Parse(x));
                var idsToReindex = new List<int>(); // ids of requested nodes and their descendant nodes

                foreach (var id in ids)
                {
                    var node = Umbraco.Content(id);
                    if (node != null)
                    {
                        _cacheService.AddToCache(id);
                        idsToReindex.Add(id);

                        if (includeDescendants)
                        {
                            var idsOfDescendants = AddToCacheDescendants(node);
                            idsToReindex.AddRange(idsOfDescendants);
                        }
                    }
                }
                index.IndexItems(_valueSetBuilder.GetValueSets(_contentService.GetByIds(idsToReindex).ToArray()));
            }

            return true;
        }

        List<int> AddToCacheDescendants(UPC.IPublishedContent node)
        {
            var descendantIds = new List<int>(); 
            foreach (var culture in node.Cultures) // iterate by culture, otherwise .Descendants() returns nodes of one (random?) culture
            {
                var descendantsByCulture = node.Descendants(culture.Value.Culture).ToList();
                foreach (var descendant in descendantsByCulture.Where(x => !descendantIds.Contains(x.Id))) // ignore already processed nodes
                {
                    _cacheService.AddToCache(descendant.Id);
                    descendantIds.Add(descendant.Id);
                }
            }
            return descendantIds;
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

        [HttpGet]
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

        [HttpGet]
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

        [HttpGet]
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

        [HttpGet]
        public SearchInfo GetSearchSettings()
        {
            var info = new SearchInfo();
            var defaultSearch = new Search("");

            info.DefaultSettings = new SearchSettings()
            {
                TitleProperties = new List<string>() { _fullTextConfig.DefaultTitleField },
                BodyProperties = new List<string>() { _fullTextConfig.FullTextContentField },
                SummaryProperties = new List<string>() { _fullTextConfig.FullTextContentField },
                Culture = _localizationService.GetDefaultLanguageIsoCode(),
                RootNodeIds = new int[] { },
                SummaryLength = defaultSearch.SummaryLength,
                EnableWildcards = defaultSearch.AddWildcard,
                Fuzzyness = defaultSearch.Fuzzyness,
                TitleBoost = defaultSearch.TitleBoost,
                SearchType = defaultSearch.SearchType.ToString(),
                AllowedContentTypes = new List<string>()
            };
            info.Cultures = _localizationService.GetAllLanguages().Select(x => x.IsoCode).ToList();

            return info;
        }

        [HttpPost]
        public IndexedNodeResult GetSearchResult([FromBody]SearchRequest searchRequest)
        {
            if (searchRequest.SearchTerms.IsNullOrWhiteSpace()) return null;

            var pageNumber = searchRequest.PageNumber < 1 ? 1 : searchRequest.PageNumber;
            var search = new Search(searchRequest.SearchTerms)
                .SetPageLength(20)
                .EnableHighlighting();

            if (searchRequest.AdvancedSettings != null)
            {
                search.AddTitleProperties(searchRequest.AdvancedSettings.TitleProperties.ToArray())
                    .SetTitleBoost(searchRequest.AdvancedSettings.TitleBoost)
                    .SetSearchType((SearchType)Enum.Parse(typeof(SearchType), searchRequest.AdvancedSettings.SearchType))
                    .AddBodyProperties(searchRequest.AdvancedSettings.BodyProperties.ToArray())
                    .AddSummaryProperties(searchRequest.AdvancedSettings.SummaryProperties.ToArray())
                    .SetSummaryLength(searchRequest.AdvancedSettings.SummaryLength)
                    .AddRootNodeIds(searchRequest.AdvancedSettings.RootNodeIds)
                    .SetCulture(searchRequest.AdvancedSettings.Culture)
                    .SetFuzzyness(searchRequest.AdvancedSettings.Fuzzyness)
                    .AddAllowedContentTypes(searchRequest.AdvancedSettings.AllowedContentTypes.ToArray());
                if (searchRequest.AdvancedSettings.EnableWildcards)
                    search.EnableWildcards();
            }
            else
            {
                search.SetCulture(_localizationService.GetDefaultLanguageIsoCode());
            }

            var searchResult = _searchService.Search(search, pageNumber);

            var result = new IndexedNodeResult();
            result.PageNumber = pageNumber;
            result.TotalPages = searchResult.TotalPages;
            result.Items = searchResult.Results.Select(x =>
                new IndexedNode()
                {
                    Id = x.Id,
                    Name = x.Title,
                    Description = x.Summary.ToString(),
                    Icon = x.Fields["__Icon"],
                    AllFields = x.Fields
                }).ToList();

            return result;

        }

        private string GetBreadcrumb(string path)
        {
            var pathParts = path.Split(',').Select(int.Parse).Skip(1);
            var pathNames = pathParts.Select(x => Umbraco.Content(x)?.Name).Where(x => x != null);
            return string.Join(" / ", pathNames);
        }

        private string GetReasonForBeingIncorrect(ISearchResult searchResult)
        {
            var reasons = new List<string>();

            if (_fullTextConfig.DisallowedContentTypeAliases.InvariantContains(searchResult.GetValues("__NodeTypeAlias").FirstOrDefault()))
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
    }
}
