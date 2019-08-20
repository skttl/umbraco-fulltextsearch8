using Our.Umbraco.FullTextSearch.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Our.Umbraco.FullTextSearch.Models
{
    public class FullTextSearchResult : IFullTextSearchResult
    {
        public FullTextSearchResult()
        {
            Results = Enumerable.Empty<SearchResultItem>().ToList();
        }
        public long TotalPages { get; set; }
        public long CurrentPage { get; set; }
        public long TotalResults { get; set; }
        public IEnumerable<ISearchResultItem> Results { get; set; }
    }
}
