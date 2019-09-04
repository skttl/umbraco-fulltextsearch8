using Our.Umbraco.FullTextSearch.Interfaces;
using System.Collections.Generic;
using System.Web;

namespace Our.Umbraco.FullTextSearch.Models
{
    public class SearchResultItem : ISearchResultItem
    {
        public string Id { get; set; }
        public double Score { get; set; }
        public IReadOnlyDictionary<string, string> Fields { get; set; }
        public string Title { get; set; }
        public IHtmlString Summary { get; set; }
    }
}
