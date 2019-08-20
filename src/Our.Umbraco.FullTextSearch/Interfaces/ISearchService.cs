namespace Our.Umbraco.FullTextSearch.Interfaces
{
    public interface ISearchService
    {
        IFullTextSearchResult Search(ISearch search, int currentPage);
    }
}
