using Our.Umbraco.FullTextSearch.Interfaces;
using System.Web;
using Umbraco.Core;
using Umbraco.Web.Composing;

namespace Our.Umbraco.FullTextSearch.Helpers
{
    public static class RequestHelper
    {
        /// <summary>
        /// Check whether the current page is being rendered by the indexer
        /// </summary>
        /// <returns>true if being indexed</returns>
        public static bool IsIndexingActive(this HttpRequest request)
        {
            var config = Current.Factory.TryGetInstance(typeof(IConfig)) as IConfig;

            if (config == null) return false;

            var searchActiveStringName = config.GetByKey("SearchActiveStringName");

            return !searchActiveStringName.IsNullOrWhiteSpace() && (request.QueryString[searchActiveStringName] != null || request.Cookies[searchActiveStringName] != null);
        }

        public static bool IsIndexingActive()
        {
            return HttpContext.Current.Request.IsIndexingActive();
        }
    }
}
