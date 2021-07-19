using Microsoft.AspNetCore.Html;
using System.Collections.Generic;
using System.Web;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Our.Umbraco.FullTextSearch.Interfaces
{
    public interface ISearchResultItem
    {
        string Id { get; set; }
        double Score { get; set; }
        IReadOnlyDictionary<string, string> Fields { get; set; }
        string Title { get; set; }
        HtmlString Summary { get; set; }
        IPublishedContent Content { get; }
    }
}
