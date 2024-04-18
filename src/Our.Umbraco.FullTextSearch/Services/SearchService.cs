using Examine;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Our.Umbraco.FullTextSearch.Helpers;
using Our.Umbraco.FullTextSearch.Interfaces;
using Our.Umbraco.FullTextSearch.Models;
using Our.Umbraco.FullTextSearch.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Umbraco.Cms.Web.Common;
using Umbraco.Extensions;

namespace Our.Umbraco.FullTextSearch.Services
{
    public class SearchService : ISearchService
    {
        private readonly IExamineManager _examineManager;
        private readonly FullTextSearchOptions _options;
        private readonly ILogger<ISearchService> _logger;
        private readonly UmbracoHelper _umbracoHelper;
        private ISearch _search;
        private int _currentPage;
        private List<SearchProperty> _searchProperties;

        public SearchService(
            IExamineManager examineManager,
            IOptions<FullTextSearchOptions> options,
            ILogger<ISearchService> logger,
            UmbracoHelper umbracoHelper)
        {
            _examineManager = examineManager;
            _options = options.Value;
            _logger = logger;
            _umbracoHelper = umbracoHelper;
        }

        /// <summary>
        /// Perform the Search
        /// </summary>
        /// <param name="search">Search object containing customized search parameters, including the search terms</param>
        /// <param name="currentPage">Page of search results to return (default is first page)</param>
        /// <returns>IFullTextSearchResult object</returns>
        public IFullTextSearchResult Search(ISearch search, int currentPage = 1)
        {
            _search = search;
            SetDefaults();

            _searchProperties = SetSearchProperties();
            _currentPage = currentPage;

            var result = new FullTextSearchResult();
            var results = GetResults();

            result.CurrentPage = currentPage;
            result.TotalPages = search.PageLength > 0 && results.TotalItemCount > 0 ? (int)Math.Ceiling(results.TotalItemCount / (decimal)search.PageLength) : 1;
            result.TotalResults = results.TotalItemCount;
            result.Results = results.Select(GetFullTextSearchResultItem).ToList();

            return result;
        }

        private void SetDefaults()
        {
            if (!_search.BodyProperties.Any()) _search.BodyProperties = new[] { _options.FullTextContentField };
            if (!_search.TitleProperties.Any()) _search.TitleProperties = new[] { _options.DefaultTitleField };
            if (!_search.SummaryProperties.Any()) _search.SummaryProperties = _search.BodyProperties;
        }

        private List<SearchProperty> SetSearchProperties()
        {
            _searchProperties = new List<SearchProperty>();

            var bodyProperties = !_search.BodyProperties.Any() ? new[] { _options.FullTextContentField } : _search.BodyProperties;

            _searchProperties.AddRange(GetProperties(_search.TitleProperties, _search.TitleBoost, _search.Fuzzyness, _search.AddWildcard));
            _searchProperties.AddRange(GetProperties(bodyProperties, 1.0, _search.Fuzzyness, _search.AddWildcard));
            return _searchProperties;
        }

        private ISearchResults GetResults()
        {
            var query = new StringBuilder();
            var queryParts = new List<string>();

            if (_search.SearchTerm.IsNullOrWhiteSpace() == false)
            {
                switch (_search.SearchType)
                {
                    case SearchType.MultiRelevance:
                    case SearchType.MultiAnd:

                        // We formulate the query differently depending on the input.
                        if (_search.SearchTerm.Contains('"'))
                        {
                            // If the user has entered double quotes we don't bother
                            // searching for the full string
                            query.Append(QueryAllPropertiesOr(_search.SearchTermSplit, 1));
                        }
                        else if (!_search.SearchTerm.Contains('"') && !_search.SearchTerm.Contains(' '))
                        {
                            // if there's no spaces or quotes we don't need to get the quoted term and boost it
                            query.Append(QueryAllPropertiesOr(_search.SearchTermSplit, 1));
                        }
                        else
                        {
                            // otherwise we search first for the entire query in quotes,
                            // then for each term in the query OR'd together.
                            query.Append($"({QueryAllPropertiesOr(_search.SearchTermQuoted, 2)} OR {QueryAllPropertiesOr(_search.SearchTermSplit, 1)})");
                        }

                        break;

                    case SearchType.SimpleOr:

                        query.Append(QueryAllProperties(_search.SearchTermSplit, 1.0, "OR", true));
                        break;

                    case SearchType.AsEntered:

                        query.Append(QueryAllPropertiesAnd(_search.SearchTermSplit, 1.0));
                        break;
                }
                queryParts.Add(query.ToString());
            }

            if (_search.RootNodeIds.Any())
            {
                var rootNodeGroup = string.Join(" OR ", _search.RootNodeIds.Select(x =>
                    $"{_options.FullTextPathField}:{x}"));
                queryParts.Add(rootNodeGroup);
            }

            var allowedContentTypes = _search.AllowedContentTypes.Where(t => !string.IsNullOrWhiteSpace(t)).ToList();
            if (allowedContentTypes.Any())
            {
                var contentTypeGroup = string.Join(" OR ", allowedContentTypes.Select(x =>
                    $"__NodeTypeAlias:{x}"));
                queryParts.Add(contentTypeGroup);
            }


            if (_search.PublishedOnly)
            {
                var publishedPropertySuffix = string.IsNullOrEmpty(_search.Culture) ? "" : $"_{_search.Culture.ToLower()}";
                var publishedQuery = $"((__VariesByCulture:y AND __Published{publishedPropertySuffix}:y) OR (__VariesByCulture:n AND __Published:y))";
                queryParts.Add(publishedQuery);
            }

            if (_search.ContentOnly)
            {
                queryParts.Add("__IndexType:content");
            }

            var disallowedContentTypes = _options.DisallowedContentTypeAliases;
            if (disallowedContentTypes.Any()) queryParts.Add($"-({string.Join(" ", disallowedContentTypes.Select(x => $"__NodeTypeAlias:{x}"))})");

            var disallowedPropertyAliases = _options.DisallowedPropertyAliases;
            if (disallowedPropertyAliases.Any())
            {
                var disallowedPropertyAliasGroup = string.Join(" OR ", disallowedPropertyAliases.Select(x => $"{x}_{_search.Culture}:1 OR {x}:1"));
                queryParts.Add($"-({disallowedPropertyAliasGroup})");
            }

            if (_search.RequireTemplate)
            {
                queryParts.Add($"-(templateID:0)");
            }

            if (!string.IsNullOrWhiteSpace(_search.CustomQuery))
            {
                queryParts.Add($"({_search.CustomQuery})");
            }

            ISearcher searcher = null;

            if (string.IsNullOrWhiteSpace(_search.Searcher) || !_examineManager.TryGetSearcher(_search.Searcher, out searcher))
            {
                if (_examineManager.TryGetIndex(_search.Index, out IIndex index))
                {
                    searcher = index.Searcher;
                }
            }

            query.Clear();
            query.Append(string.Join(" AND ", queryParts.Select(x => $"({x})")));

            if (searcher != null)
            {
                _logger.LogDebug("Trying to search for {query}", query.ToString());

                var searchQuery = searcher.CreateQuery().NativeQuery(query.ToString());
                var queryOptions = new Examine.Search.QueryOptions(_search.PageLength * (_currentPage - 1), _search.PageLength);

                if (_search.OrderByFields?.Length > 0)
                {
                    if (_search.OrderDirection is OrderDirection.Descending)
                    {
                        return searchQuery.OrderByDescending(_search.OrderByFields).Execute(queryOptions);
                    }
                    else
                    {
                        return searchQuery.OrderBy(_search.OrderByFields).Execute(queryOptions);
                    }
                }
                else
                {
                    return searchQuery.Execute(queryOptions);
                }
            }

            return null;
        }

        /// <summary>
        /// OR's together all the passed search terms into a query
        /// for each property in the properties list
        ///
        /// </summary>
        /// <param name="searchTerms">A list of fully escaped search terms</param>
        /// <param name="boostAll">all terms are boosted by this amount, multiplied by the amount in the property/boost dictionary</param>
        /// <returns>a query fragment</returns>
        protected StringBuilder QueryAllPropertiesOr(ICollection<string> searchTerms, double boostAll)
        {
            if (searchTerms == null || searchTerms.Count < 1)
                return new StringBuilder();

            return QueryAllProperties(searchTerms, boostAll, "OR");
        }

        /// <summary>
        /// AND's together all the passed search terms into a query
        /// for each property in the properties list
        /// </summary>
        /// <param name="searchTerms">A list of fully escaped search terms</param>
        /// <param name="boostAll">all terms are boosted by this amount, multiplied by the amount in the property/boost dictionary</param>
        /// <returns>a query fragment</returns>
        protected StringBuilder QueryAllPropertiesAnd(ICollection<string> searchTerms, double boostAll)
        {
            if (searchTerms == null || searchTerms.Count < 1)
                return new StringBuilder();

            return QueryAllProperties(searchTerms, boostAll, "AND");
        }

        /// <summary>
        /// Called by queryAllPropertiesOr, queryAllPropertiesAnd
        /// Creates a somewhat convuleted lucene query string.
        /// Each search term is applied to each property in the umbracoProperties list,
        /// boosted by the boost value associated with the property, multiplied by
        /// the boost value passed to the function.
        /// The global fuzziness level is applied, multiplied by the fuzzyness value
        /// associated with the relevant property.
        /// Terms are ether OR'd or AND'd (or theoretically anything else
        /// you stick into joinWith'd, though I can't think of much that would
        /// actually be useful) according to the contents of joinWith
        /// </summary>
        /// <param name="searchTerms">A list of fully escaped search terms</param>
        /// <param name="boostAll">Boost all terms by this amount</param>
        /// <param name="joinWith">Join terms with this string, should be AND/OR</param>
        /// <param name="simplify"></param>
        /// <returns></returns>
        protected StringBuilder QueryAllProperties(ICollection<string> searchTerms, double boostAll, string joinWith, bool simplify = false)
        {
            var queryBuilder = new List<StringBuilder>();
            foreach (var term in searchTerms)
            {
                var termQuery = new StringBuilder();
                foreach (var property in _searchProperties)
                {
                    termQuery.Append(simplify
                                         ? QuerySingleItemSimple(term.Trim(), property)
                                         : QuerySingleItem(term.Trim(), property, boostAll));
                }
                if (termQuery.Length > 0)
                    queryBuilder.Add(termQuery);
            }
            var query = new StringBuilder();
            var count = queryBuilder.Count;
            if (count < 1)
                return query;
            var i = 0;
            for (; ; )
            {
                query.Append($" ({queryBuilder[i]}) ");
                if (++i >= count)
                    break;
                query.Append($"{joinWith} ");
            }
            return query;
        }
        protected string QuerySingleItem(string term, SearchProperty property, double boostAll)
        {
            var boost = property.BoostMultiplier * boostAll;
            var boostString = string.Empty;
            if (boost != 1.0)
            {
                boostString = "^" + boost.ToString(CultureInfo.InvariantCulture);
            }
            var fuzzyString = string.Empty;
            var wildcardQuery = string.Empty;
            if (!term.Contains('"'))
            {
                // wildcard queries get lower relevance than exact matches, and ignore fuzzieness
                if (property.Wildcard)
                {
                    var halfBoost = (boost * 0.5).ToString(CultureInfo.InvariantCulture);
                    wildcardQuery = $"({property.PropertyName}:{term}*^{halfBoost} OR {property.PropertyName}_{_search.Culture}:{term}*^{halfBoost}) ";
                }
                else
                {
                    var fuzzyLocal = property.FuzzyMultiplier;
                    if (fuzzyLocal < 1.0 && fuzzyLocal > 0.0)
                    {
                        fuzzyString = "~" + fuzzyLocal.ToString(CultureInfo.InvariantCulture);
                    }
                }
            }
            return $"({property.PropertyName}:{term}{fuzzyString}{boostString} OR {property.PropertyName}_{_search.Culture}:{term}{fuzzyString}{boostString}) {wildcardQuery}";
        }
        protected string QuerySingleItemSimple(string term, SearchProperty property)
        {
            var fuzzyString = string.Empty;
            var wildcard = string.Empty;
            if (!term.Contains('"'))
            {
                if (property.Wildcard)
                {
                    wildcard = "*";
                }
                else
                {
                    var fuzzyLocal = property.FuzzyMultiplier;
                    if (fuzzyLocal < 1.0 && fuzzyLocal > 0.0)
                    {
                        fuzzyString = "~" + fuzzyLocal.ToString(CultureInfo.InvariantCulture);
                    }
                }
            }
            return $"({property.PropertyName}:{term}{fuzzyString}{wildcard} OR {property.PropertyName}_{_search.Culture}:{term}{fuzzyString}{wildcard}) ";
        }

        /// <summary>
        /// Split up the comma separated string and return a list of UmbracoProperty objects
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="boost"></param>
        /// <param name="fuzzy"></param>
        /// <param name="wildcard"></param>
        /// <returns></returns>
        private static IEnumerable<SearchProperty> GetProperties(ICollection<string> properties, double boost, double fuzzy, bool wildcard)
        {
            var umbracoProperties = new List<SearchProperty>();
            if (properties.Any())
            {
                umbracoProperties.AddRange(properties.Where(propName => !string.IsNullOrEmpty(propName))
                    .Select(propName => new SearchProperty(propName, boost, fuzzy, wildcard)));
            }
            return umbracoProperties;
        }

        public SearchResultItem GetFullTextSearchResultItem(ISearchResult result)
        {
            var item = new SearchResultItem(_umbracoHelper)
            {
                Id = result.Id,
                Fields = result.Values,
                Score = result.Score,
                Title = GetTitle(result),
                Summary = new HtmlString(GetSummary(result))
            };

            return item;
        }

        public string GetTitle(ISearchResult result)
        {
            var title = string.Empty;
            var props = _search.TitleProperties.SelectMany(x => new[] { $"{x}_{_search.Culture}", x });
            foreach (var prop in props)
            {
                title = result.GetValues(prop).FirstOrDefault();
                if (!title.IsNullOrWhiteSpace()) break;
            }
            return title;
        }

        public string GetSummary(ISearchResult result)
        {
            var summary = string.Empty;
            var props = _search.SummaryProperties.Concat(_search.BodyProperties).SelectMany(x => new[] { $"{x}_{_search.Culture}", x });
            foreach (var prop in props)
            {
                summary = result.GetValues(prop).FirstOrDefault();
                if (summary.IsNullOrWhiteSpace()) continue;

                summary = SummarizeText(summary);
                break;
            }
            return summary;
        }

        protected string SummarizeText(string input)
        {
            string summary;
            if (!input.IsNullOrWhiteSpace() && (input.Length > _search.SummaryLength || _search.HighlightSearchTerms))
            {
                summary = Highlighter.FindSnippet(input, string.Join(" ", _search.SearchTermSplit), _search.SummaryLength, _search.HighlightSearchTerms ? _options.HighlightPattern : "{0}");

                if (!string.IsNullOrEmpty(summary))
                    return summary;
            }

            summary = input.TruncateHtml(_search.SummaryLength, "&hellip;");

            return summary;
        }
    }
}
