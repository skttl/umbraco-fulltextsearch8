using Examine;

namespace Our.Umbraco.FullTextSearch.Interfaces
{
    public interface IStatusService
    {
        bool TryGetIncorrectIndexedNodes(out ISearchResults results, int maxResults = int.MaxValue);
        bool TryGetIndexableNodes(out ISearchResults results, int maxResults = int.MaxValue);
        bool TryGetIndexedNodes(out ISearchResults results, int maxResults = int.MaxValue);
        bool TryGetMissingNodes(out ISearchResults results, int maxResults = int.MaxValue);
    }
}