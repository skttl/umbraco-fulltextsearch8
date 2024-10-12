using System.Collections.Generic;

namespace Our.Umbraco.FullTextSearch.Interfaces;

public interface IFullTextSearchResult
{
    long TotalPages { get; set; }
    long CurrentPage { get; set; }
    long TotalResults { get; set; }
    IEnumerable<ISearchResultItem> Results { get; set; }
}
