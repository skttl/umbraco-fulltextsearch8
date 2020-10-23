using Our.Umbraco.FullTextSearch.Interfaces;
using Our.Umbraco.FullTextSearch.Models;
using System.Web;
using Umbraco.Core;
using Umbraco.Web.Composing;

namespace Our.Umbraco.FullTextSearch.Helpers
{
    public static class FullTextSearchHelper
    {
        /// <summary>
        /// Check whether the current page is being rendered by the indexer
        /// </summary>
        /// <returns>true if being indexed</returns>
        public static bool IsIndexingActive(this HttpRequestBase request)
        {
            if (!(Current.Factory.TryGetInstance(typeof(IFullTextSearchConfig)) is IFullTextSearchConfig config)) return false;

            var searchActiveStringName = config.IndexingActiveKey;

            return !searchActiveStringName.IsNullOrWhiteSpace() && request.RequestContext.HttpContext.Items[searchActiveStringName] != null;
        }

        /// <summary>
        /// Perform a search using the default search settings
        /// </summary>
        /// <param name="searchTerms"></param>
        /// <param name="culture"></param>
        /// <param name="currentPage"></param>
        /// <returns></returns>
        public static IFullTextSearchResult Search(string searchTerms, string culture = null, int currentPage = 1)
        {
            if (!(Current.Factory.TryGetInstance(typeof(ISearchService)) is ISearchService searchService)) return null;

            currentPage = currentPage < 1 ? 1 : currentPage;

            var search = new Search(searchTerms);

            if (!culture.IsNullOrWhiteSpace())
                search = search.SetCulture(culture);

            return searchService.Search(search, currentPage);
        }

        /// <summary>
        /// Perform a search using a search settings object
        /// </summary>
        /// <param name="search"></param>
        /// <param name="currentPage"></param>
        /// <returns></returns>
        public static IFullTextSearchResult Search(Search search, int currentPage = 1)
        {
            if (!(Current.Factory.TryGetInstance(typeof(ISearchService)) is ISearchService searchService)) return null;

            currentPage = currentPage < 1 ? 1 : currentPage;

            return searchService.Search(search, currentPage);
        }
    }
}
