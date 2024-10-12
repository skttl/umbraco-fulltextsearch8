using Examine;

namespace Our.Umbraco.FullTextSearch.Interfaces
{
    public interface IStatusService
    {
        bool TryGetIncorrectIndexedNodes(out ISearchResults results, int maxResults = int.MaxValue);
        bool TryGetMissingNodes(out ISearchResults results, int maxResults = int.MaxValue);
    }
}