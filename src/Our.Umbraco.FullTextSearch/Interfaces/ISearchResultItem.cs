using System.Collections.Generic;
using System.Web;

namespace Our.Umbraco.FullTextSearch.Interfaces
{
    public interface ISearchResultItem
    {
        string Id { get; set; }
        double Score { get; set; }
        IReadOnlyDictionary<string, string> Fields { get; set; }
        string Title { get; set; }
        IHtmlString Summary { get; set; }
    }
}
