using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Our.Umbraco.FullTextSearch.Interfaces;
using Our.Umbraco.FullTextSearch.Options;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Umbraco.Cms.Core.Logging;

namespace Our.Umbraco.FullTextSearch.Services;

public class HtmlService : IHtmlService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly FullTextSearchOptions _options;
    private readonly ILogger _logger;
    private readonly IProfilingLogger _profilingLogger;

    public HtmlService(
        IHttpContextAccessor httpContextAccessor,
        IOptions<FullTextSearchOptions> options,
        ILogger<IHtmlService> logger,
        IProfilingLogger profilingLogger)
    {
        _httpContextAccessor = httpContextAccessor;
        _options = options.Value;
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
            _logger.LogError(ex, "RemoveByXpaths exception.");
            // swallow the exception and run the regex based tag stripper on it anyway. Won't be perfect but better than nothing.
            return fullHtml;
        }

        if (_options.XPathsToRemove != null)
        {
            foreach (var xPath in _options.XPathsToRemove)
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
