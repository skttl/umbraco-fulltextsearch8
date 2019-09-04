using Examine;
using Umbraco.Core.Logging;
using Our.Umbraco.FullTextSearch.Interfaces;
using Our.Umbraco.FullTextSearch.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Umbraco.Examine;
using Umbraco.Web;
using System;
using Umbraco.Core;
using Our.Umbraco.FullTextSearch.Helpers;
using System.Web;

namespace Our.Umbraco.FullTextSearch.Services
{
    public class SearchService : ISearchService
    {
        private readonly IExamineManager _examineManager;
        private readonly IConfig _fullTextConfig;
        private readonly ILogger _logger;
        private ISearch _search;
        private int _currentPage;
        List<SearchProperty> _searchProperties;

        public SearchService(IExamineManager examineManager, IConfig fullTextConfig, ILogger logger)
        {
            _examineManager = examineManager;
            _fullTextConfig = fullTextConfig;
            _logger = logger;
        }
        public IFullTextSearchResult Search(ISearch search, int currentPage = 1)
        {
            _search = search;
            SetDefaults();

            _searchProperties = SetSearchProperties();
            _currentPage = currentPage;

            var result = new FullTextSearchResult();
            var results = GetResults();

            result.CurrentPage = currentPage;
            result.TotalPages = search.PageLength > 0 && results.TotalItemCount > 0 ? (int)Math.Ceiling((decimal)results.TotalItemCount / (decimal)search.PageLength) : 1;
            result.TotalResults = results.TotalItemCount;
            result.Results = (search.PageLength > 0 ? results.Skip(search.PageLength * (currentPage - 1)) : results).Select(x => GetFullTextSearchResultItem(x));

            return result;
        }

        private void SetDefaults()
        {
            if (!_search.BodyProperties.Any()) _search.BodyProperties = new string[] { _fullTextConfig.GetFullTextFieldName() };
            if (!_search.TitleProperties.Any()) _search.TitleProperties = new string[] { _fullTextConfig.GetDefaultTitleFieldName() };
            if (!_search.SummaryProperties.Any()) _search.SummaryProperties = _search.BodyProperties;
        }

        private List<SearchProperty> SetSearchProperties()
        {
            _searchProperties = new List<SearchProperty>();
            var titleBoost = _fullTextConfig.GetSearchTitleBoost();

            var bodyProperties = !_search.BodyProperties.Any() ? new string[] { _fullTextConfig.GetFullTextFieldName() } : _search.BodyProperties;

            _searchProperties.AddRange(GetProperties(_search.TitleProperties, titleBoost, _search.Fuzzyness, _search.AddWildcard));
            _searchProperties.AddRange(GetProperties(bodyProperties, 1.0, _search.Fuzzyness, _search.AddWildcard));
            return _searchProperties;
        }

        private ISearchResults GetResults()
        {

            if (_examineManager.TryGetIndex("ExternalIndex", out var index))
            {
                var query = new StringBuilder();

                query.Append("(");

                switch (_search.SearchType)
                {
                    case SearchType.MultiRelevance:

                        // We formulate the query differently depending on the input.
                        if (_search.SearchTerm.Contains('"'))
                        {
                            // If the user has enetered double quotes we don't bother 
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
                            query.AppendFormat("({0} OR {1})",
                                QueryAllPropertiesOr(_search.SearchTermQuoted, 2)
                                , QueryAllPropertiesOr(_search.SearchTermSplit, 1)
                            );
                        }

                        break;

                    case SearchType.MultiAnd:

                        if (_search.SearchTerm.Contains('"'))
                        {
                            // If the user has enetered double quotes we don't bother 
                            // searching for the full string
                            query.Append(QueryAllPropertiesAnd(_search.SearchTermSplit, 1.0));
                        }
                        else if (!_search.SearchTerm.Contains('"') && !_search.SearchTerm.Contains(' '))
                        {
                            // if there's no spaces or quotes we don't need to get the quoted term and boost it
                            query.Append(QueryAllPropertiesAnd(_search.SearchTermSplit, 1));
                        }
                        else
                        {
                            // otherwise we search first for the entire query in quotes, 
                            // then for each term in the query OR'd together.
                            query.AppendFormat("{0} OR {1}",
                                QueryAllPropertiesAnd(_search.SearchTermQuoted, 2)
                                , QueryAllPropertiesAnd(_search.SearchTermSplit, 1)
                            );
                        }
                        break;

                    case SearchType.SimpleOr:

                        query.Append(QueryAllProperties(_search.SearchTermSplit, 1.0, "OR", true));
                        break;

                    case SearchType.AsEntered:

                        query.Append(QueryAllPropertiesAnd(_search.SearchTermSplit, 1.0));
                        break;
                }
                query.Append(")");

                if (_search.RootNodeIds.Any())
                {
                    var pathName = _fullTextConfig.GetPathFieldName();
                    var rootNodeGroup = string.Join(" OR ", _search.RootNodeIds.Select(x => string.Format("{0}:{1}", pathName, x.ToString())));
                    query.AppendFormat(" AND ({0})", rootNodeGroup);
                }

                query.Append($" AND (__IndexType:content AND __Published_{_search.Culture}:y)");

                var searcher = index.GetSearcher();
                _logger.Info<SearchService>("Trying to search for {query}", query.ToString());
                return searcher.CreateQuery().NativeQuery(query.ToString()).Execute(_search.PageLength * _currentPage);
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
                                         ? QuerySingleItemSimple(term, property)
                                         : QuerySingleItem(term, property, boostAll));
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
                query.AppendFormat(" ({0}) ", queryBuilder[i]);
                if (++i >= count)
                    break;
                query.AppendFormat("{0} ", joinWith);
            }
            return query;
        }
        protected string QuerySingleItem(string term, SearchProperty property, double boostAll)
        {
            var boost = property.BoostMultiplier * boostAll;
            var boostString = string.Empty;
            if (boost != 1.0)
            {
                boostString = "^" + boost;
            }
            var fuzzyString = string.Empty;
            var wildcardQuery = string.Empty;
            if (!term.Contains('"'))
            {
                // wildcard queries get lower relevance than exact matches, and ignore fuzzieness
                if (property.Wildcard)
                {
                    wildcardQuery = string.Format(System.Globalization.CultureInfo.InvariantCulture, "({0}:{1}*^{2} OR {0}_{4}:{1}*^{2}) ", property.PropertyName, term, boost * 0.5, _search.Culture);
                }
                else
                {
                    double fuzzyLocal = property.FuzzyMultiplier;
                    if (fuzzyLocal < 1.0 && fuzzyLocal > 0.0)
                    {
                        fuzzyString = "~" + fuzzyLocal;
                    }
                }
            }
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "({0}:{1}{2}{3} OR {0}_{5}:{1}{2}{3}) {4}", property.PropertyName, term, fuzzyString, boostString, wildcardQuery, _search.Culture);
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
                        fuzzyString = "~" + fuzzyLocal;
                    }
                }
            }
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "({0}:{1}{2}{3} OR {0}_{4}:{1}{2}{3}) ", property.PropertyName, term, fuzzyString, wildcard, _search.Culture);
        }

        /// <summary>
        /// Split up the comma separated string and retun a list of UmbracoProperty objects
        /// </summary>
        /// <param name="commaSeparated"></param>
        /// <param name="boost"></param>
        /// <param name="fuzzy"></param>
        /// <param name="wildcard"></param>
        /// <returns></returns>
        static List<SearchProperty> GetProperties(string[] properties, double boost, double fuzzy, bool wildcard)
        {
            var umbracoProperties = new List<SearchProperty>();
            if (properties.Any())
            {
                foreach (var propName in properties)
                {
                    if (!string.IsNullOrEmpty(propName))
                    {
                        umbracoProperties.Add(new SearchProperty(propName, boost, fuzzy, wildcard));
                    }
                }
            }
            return umbracoProperties;
        }

        public SearchResultItem GetFullTextSearchResultItem(ISearchResult result)
        {
            var item = new SearchResultItem
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
            var props = _search.TitleProperties.SelectMany(x => new string[] { $"{x}_{_search.Culture}", x });
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
            var props = _search.SummaryProperties.Concat(_search.BodyProperties).SelectMany(x => new string[] { $"{x}_{_search.Culture}", x });
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
            var summaryBuilder = new StringBuilder();
            var summary = "";
            if (!input.IsNullOrWhiteSpace() && (input.Length > _search.SummaryLength || _search.HighlightSearchTerms))
            {
                //(\\S*.{0,10})?("+ queryString +")(.{0,10}\\S*)?
                var searchTerm = "";
                if (_search.SearchTerm.Contains('"'))
                {
                    searchTerm = string.Join("|", _search.SearchTermSplit);
                }
                else if (!_search.SearchTerm.Contains('"') && !_search.SearchTerm.Contains(' '))
                {
                    searchTerm = string.Join("|", _search.SearchTermSplit);
                }
                else
                {
                    searchTerm = $"{_search.SearchTerm}|{string.Join("|", _search.SearchTermSplit)}";
                }
                var matches = Regex.Matches(input, @"(\S*.{0,20})(" + searchTerm + @")(.{0,20}\S*)?", RegexOptions.IgnoreCase);

                if (matches.Count == 0)
                {
                    summaryBuilder.Append(input);
                }
                else
                {
                    foreach (Match match in matches)
                    {
                        if (match.Groups.Count > 3)
                        {
                            if (match.Index > 0 && !match.Groups[1].Value.IsNullOrWhiteSpace()) summaryBuilder.Append($" &hellip;{match.Groups[1].Value}");

                            if (_search.HighlightSearchTerms) summaryBuilder.Append($"<b>{match.Groups[2].Value}</b>");
                            else summaryBuilder.Append(match.Groups[2].Value);

                            if (!match.Groups[3].Value.IsNullOrWhiteSpace()) summaryBuilder.Append($"{match.Groups[3].Value}&hellip; ");
                        }
                    }
                }

                summary = summaryBuilder.ToString().TruncateHtml(_search.SummaryLength, "&hellip;");
            }
            else
            {
                summary = input.TruncateHtml(_search.SummaryLength, "&hellip;");
            }
            return summary;
        }
    }
}
