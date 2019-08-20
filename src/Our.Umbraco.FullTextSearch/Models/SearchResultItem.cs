using Our.Umbraco.FullTextSearch.Interfaces;
using System.Collections.Generic;

namespace Our.Umbraco.FullTextSearch.Models
{
    public class SearchResultItem : ISearchResultItem
    {
        public string Id { get; set; }
        public double Score { get; set; }
        public IReadOnlyDictionary<string, string> Fields { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Url { get; set; }
    }
}
