using HtmlAgilityPack;
using Our.Umbraco.FullTextSearch.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Umbraco.Core.Logging;

namespace Our.Umbraco.FullTextSearch.Services
{
    public class HtmlService : IHtmlService
    {
        private readonly IConfig _fullTextConfig;
        private readonly ILogger _logger;
        private readonly IProfilingLogger _profilingLogger;

        public HtmlService(IConfig fullTextConfig, ILogger logger, IProfilingLogger profilingLogger)
        {
            _fullTextConfig = fullTextConfig;
            _logger = logger;
            _profilingLogger = profilingLogger;
        }

        public bool GetHtmlByUrl(string url, out string fullHtml)
        {
            using (_profilingLogger.DebugDuration<HtmlService>("GetHtmlByUrl(" + url + ")", "GetHtmlByUrl(" + url + ") done"))
            {


                if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
                {
                    fullHtml = "";
                    return false;
                }
                
                try
                {
                    var httpHost = _fullTextConfig.GetHttpHost();
                    var httpTimeout = _fullTextConfig.GetHttpTimeout();

                    var cookieDictionary = GetQueryStringCollection();
                    var webRequest = (HttpWebRequest)WebRequest.Create(uri);
                    httpTimeout *= 1000;
                    webRequest.Timeout = httpTimeout;
                    webRequest.UserAgent = "FullTextIndexer";

                    if (cookieDictionary != null && cookieDictionary.Count > 0)
                    {
                        var container = new CookieContainer();
                        var domain = webRequest.Address.DnsSafeHost;
                        foreach (var cookie in cookieDictionary)
                        {
                            container.Add(new Cookie(cookie.Key, cookie.Value, "/", domain));
                        }
                        webRequest.CookieContainer = container;
                    }
                    var webResponse = (HttpWebResponse)webRequest.GetResponse();
                    using (var sr = new StreamReader(webResponse.GetResponseStream()))
                    {
                        fullHtml = sr.ReadToEnd();
                    }
                    return true;
                }
                catch (WebException ex)
                {
                    _logger.Error<HtmlService>(ex, "Error in FullTextSearch retrieval.");
                    fullHtml = string.Empty;
                }
                return false;
            }
        }

        /// <summary>
        /// Gets the name of the query string variable to pass to rendered pages from the config, and sticks it
        /// into a dictionary. Hardly needs it's own method, but it's used in a few places so...
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetQueryStringCollection()
        {
            var queryString = new Dictionary<string, string>();
            var getStringName = _fullTextConfig.GetSearchActiveStringName();;
            if (!string.IsNullOrWhiteSpace(getStringName))
            {
                queryString.Add(getStringName, "1");
            }
            return queryString;
        }

        public string GetTextFromHtml(string fullHtml)
        {
            using (_profilingLogger.DebugDuration<HtmlService>("GetTextFromHtml", "GetTextFromHtml done"))
            {
                if (fullHtml.Length < 1)
                {
                    return "";
                }
                var fullText = RemoveByXpaths(fullHtml);

                // the above leaves some residual tags. Search me why, probably the HTML input isn't perfectly
                // formed and the parser is choking on it. Help it along
                fullText = Regex.Replace(fullText, "<[^>]*>", String.Empty);

                // Decode any HTML entities
                fullText = HttpUtility.HtmlDecode(fullText);

                // replace multiple spaces with single spaces. 
                fullText = Regex.Replace(fullText, @"(\s)(\s+)", "$1", RegexOptions.Singleline);
                return fullText;
            }
        }

        private string RemoveByXpaths(string fullHtml)
        {
            var doc = new HtmlDocument();
            try
            {
                //Does this break stuff?
                doc.OptionReadEncoding = false;
                doc.LoadHtml(fullHtml);
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) HttpContext.Current.Trace.Warn("Search", "There was an exception cleaning HTML: " + ex);

                _logger.Error(GetType(), "RemoveByXpaths exception.", ex);
                // swallow the exception and run the regex based tag stripper on it anyway. Won't be perfect but better than nothing. 
                return fullHtml;
            }

            var xPathsToRemove = _fullTextConfig.GetXpathsToRemoveFromFullText();
            foreach (var xPath in xPathsToRemove)
            {
                var nodes = doc.DocumentNode.SelectNodes(xPath);
                if (nodes != null)
                {
                    foreach (var h in nodes)
                    {
                        h.ParentNode.RemoveChild(h, false);
                    }
                }
            }

            var fullTextBuilder = new StringBuilder();
            TagStripHTML(doc.DocumentNode, fullTextBuilder);
            return fullTextBuilder.ToString();
        }

        private void TagStripHTML(HtmlNode root, StringBuilder fullTextBuilder)
        {
            if (root.HasChildNodes)
            {
                foreach (var node in root.ChildNodes)
                {
                    TagStripHTML(node, fullTextBuilder);
                }
            }
            else if (root.NodeType == HtmlNodeType.Text)
            {
                fullTextBuilder.Append(root.InnerText);
                fullTextBuilder.Append(" ");
            }
        }
    }
}
