using System.Collections.Generic;

namespace Our.Umbraco.FullTextSearch.Interfaces
{
    public interface ISearchResultItem
    {
        string Id { get; set; }
        double Score { get; set; }
        IReadOnlyDictionary<string, string> Fields { get; set; }
        string Title { get; set; }
        string Summary { get; set; }
        string Url { get; set; }
    }
}
