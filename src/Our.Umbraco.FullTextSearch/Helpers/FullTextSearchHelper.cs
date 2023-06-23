using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Our.Umbraco.FullTextSearch.Interfaces;
using Our.Umbraco.FullTextSearch.Models;
using Our.Umbraco.FullTextSearch.Options;
using Umbraco.Extensions;

namespace Our.Umbraco.FullTextSearch.Helpers
{
    public class FullTextSearchHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISearchService _searchService;
        private readonly FullTextSearchOptions _options;

        public FullTextSearchHelper(
            IHttpContextAccessor httpContextAccessor,
            ISearchService searchService,
            IOptions<FullTextSearchOptions> options
            )
        {
            _httpContextAccessor = httpContextAccessor;
            _searchService = searchService;
            _options = options.Value;
        }
        /// <summary>
        /// Check whether the current page is being rendered by the indexer
        /// </summary>
        /// <returns>true if being indexed</returns>
        public bool IsIndexingActive()
        {
            return _httpContextAccessor.GetRequiredHttpContext().Request.Headers.UserAgent == FullTextSearchConstants.HttpClientFactoryNamedClientName;
        }

        /// <summary>
        /// Perform a search using the default search settings
        /// </summary>
        /// <param name="searchTerms"></param>
        /// <param name="culture"></param>
        /// <param name="currentPage"></param>
        /// <returns></returns>
        public IFullTextSearchResult Search(string searchTerms, string culture = null, int currentPage = 1)
        {
            currentPage = currentPage < 1 ? 1 : currentPage;

            var search = new Search(searchTerms);

            if (!culture.IsNullOrWhiteSpace())
                search = search.SetCulture(culture);

            return _searchService.Search(search, currentPage);
        }

        /// <summary>
        /// Perform a search using a search settings object
        /// </summary>
        /// <param name="search"></param>
        /// <param name="currentPage"></param>
        /// <returns></returns>
        public IFullTextSearchResult Search(Search search, int currentPage = 1)
        {
            currentPage = currentPage < 1 ? 1 : currentPage;

            return _searchService.Search(search, currentPage);
        }
    }
}
