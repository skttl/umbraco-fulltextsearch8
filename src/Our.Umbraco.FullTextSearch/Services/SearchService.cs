using Examine;
using Our.Umbraco.FullTextSearch.Interfaces;
using Our.Umbraco.FullTextSearch.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Our.Umbraco.FullTextSearch.Services
{
    public class SearchService : ISearchService
    {
        private readonly IExamineManager _examineManager;
        private readonly IConfig _fullTextConfig;
        private ISearch _search;
        private int _currentPage;
        List<SearchProperty> _searchProperties;

        public SearchService(IExamineManager examineManager, IConfig fullTextConfig)
        {
            _examineManager = examineManager;
            _fullTextConfig = fullTextConfig;
        }
        public IFullTextSearchResult Search(ISearch search, int currentPage = 1)
        {
            _search = search;
            _searchProperties = SetSearchProperties();
            _currentPage = currentPage;

            var result = new Models.FullTextSearchResult();
            var results = GetResults();

            result.CurrentPage = currentPage;
            result.TotalPages = results.TotalItemCount / search.PageLength;
            result.TotalResults = results.TotalItemCount;
            result.Results = results.Skip(search.PageLength * (currentPage - 1)).Select(x => GetFullTextSearchResultItem(x));

            return result;
        }

        private List<SearchProperty> SetSearchProperties()
        {
            _searchProperties = new List<SearchProperty>();
            var titleBoost = _fullTextConfig.GetSearchTitleBoost();
            _searchProperties.AddRange(GetProperties(_search.TitleProperties, titleBoost, _search.Fuzzyness, _search.AddWildcard));
            _searchProperties.AddRange(GetProperties(_search.BodyProperties, 1.0, _search.Fuzzyness, _search.AddWildcard));
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
                var searcher = index.GetSearcher();
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
                    wildcardQuery = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}:{1}*^{2} ", property.PropertyName, term, boost * 0.5);
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
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}:{1}{2}{3} {4}", property.PropertyName, term, fuzzyString, boostString, wildcardQuery);
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
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}:{1}{2}{3} ", property.PropertyName, term, fuzzyString, wildcard);
        }

        /// <summary>
        /// Call Examine to execute the generated query
        /// </summary> 
        /// <param name="query">query to execute</param>
        /// <returns>ISearchResults object or null</returns>
        protected ISearchResults ExecuteSearch(StringBuilder query, int maxResults = 500)
        {
            return null;
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
            var item = new Models.SearchResultItem();
            item.Id = result.Id;
            item.Fields = result.Values;
            item.Score = result.Score;
            item.Title = GetTitle(result);
            item.Summary = GetSummary(result);

            return item;
        }

        public string GetTitle(ISearchResult result)
        {
            var title = string.Empty;
            foreach (var prop in _search.TitleProperties.Where(prop => result.GetValues(prop).Any()))
            {
                title = result.GetValues(prop).FirstOrDefault();
                if (!string.IsNullOrEmpty(title)) break;
            }
            return title;
        }

        public string GetSummary(ISearchResult result)
        {
            var summary = string.Empty;
            foreach (var prop in _search.BodyProperties.Where(prop => result.GetValues(prop).Any()))
            {
                summary = GetSummaryText(result, prop);
                if (!string.IsNullOrEmpty(summary)) break;
            }
            return summary;
        }

        protected string GetSummaryText(ISearchResult result, string propertyName)
        {
            string summary;
            var value = result.GetValues(propertyName).FirstOrDefault();
            if (value.Length > _search.SummaryLength)
            {
                summary = value.Substring(0, _search.SummaryLength);
                summary = Regex.Replace(summary, @"\S*$", string.Empty, RegexOptions.Compiled);
            }
            else
            {
                summary = value;
            }
            return summary;
        }
    }
}
