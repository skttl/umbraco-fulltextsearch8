using HtmlAgilityPack;
using Our.Umbraco.FullTextSearch.Interfaces;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Umbraco.Core.Logging;

namespace Our.Umbraco.FullTextSearch.Services
{
    public class HtmlService : IHtmlService
    {
        private readonly FullTextSearchConfig _fullTextConfig;
        private readonly ILogger _logger;
        private readonly IProfilingLogger _profilingLogger;

        public HtmlService(FullTextSearchConfig fullTextConfig, ILogger logger, IProfilingLogger profilingLogger)
        {
            _fullTextConfig = fullTextConfig;
            _logger = logger;
            _profilingLogger = profilingLogger;
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

                _logger.Error<HtmlService>(ex, "RemoveByXpaths exception.");
                // swallow the exception and run the regex based tag stripper on it anyway. Won't be perfect but better than nothing.
                return fullHtml;
            }

            var xPathsToRemove = _fullTextConfig.XPathsToRemove;
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
