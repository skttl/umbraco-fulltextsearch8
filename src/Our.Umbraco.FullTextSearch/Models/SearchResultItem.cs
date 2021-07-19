using Microsoft.AspNetCore.Html;
using Our.Umbraco.FullTextSearch.Interfaces;
using System.Collections.Generic;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common;

namespace Our.Umbraco.FullTextSearch.Models
{
    public class SearchResultItem : ISearchResultItem
    {
        private UmbracoHelper _umbracoHelper;
        private IPublishedContent? _content;

        public SearchResultItem(UmbracoHelper umbracoHelper)
        {
            _umbracoHelper = umbracoHelper;
        }
        public string Id { get; set; }
        public double Score { get; set; }
        public IReadOnlyDictionary<string, string> Fields { get; set; }
        public string Title { get; set; }
        public HtmlString Summary { get; set; }
        public IPublishedContent Content
        {
            get
            {
                // Lazy load the property value and ensure it's not re-resolved once it's loaded
                return _content ?? (_content = _umbracoHelper?.Content(Id));
            }
        }
    }
}
