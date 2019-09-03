using Our.Umbraco.FullTextSearch.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web.Composing;

namespace Our.Umbraco.FullTextSearch.Helpers
{
    public static class SearchResultItemExtensions
    {
        public static string Url(this ISearchResultItem item, string culture = null)
        {
            return Current.AppCaches.RequestCache.Get($"FullTextSearch.Url.{item.Id}.{culture}", () =>
            {
                if (!int.TryParse(item.Id, out int id)) return string.Empty;
                return Current.UmbracoContext.Url(id, culture);
            }) as string;
        }
        public static string Url(this ISearchResultItem item, UrlMode mode, string culture = null)
        {
            return Current.AppCaches.RequestCache.Get($"FullTextSearch.Url.{item.Id}.{mode.ToString()}.{culture}", () =>
            {
                if (!int.TryParse(item.Id, out int id)) return string.Empty;
                return Current.UmbracoContext.Url(id, mode, culture);
            }) as string;
        }

        public static IPublishedContent Content(this ISearchResultItem item)
        {
            return (IPublishedContent)Current.AppCaches.RequestCache.Get($"FullTextSearch.Content.{item.Id}", () =>
            {
                if (!int.TryParse(item.Id, out int id)) return null;
                return Current.UmbracoHelper.Content(id);
            });
        }
    }
}
