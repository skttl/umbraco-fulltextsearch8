namespace Our.Umbraco.FullTextSearch.Interfaces;

public interface ISearchService
{
    string GetLuceneQuery(ISearch search);
    IFullTextSearchResult Search(ISearch search, int currentPage);
}
