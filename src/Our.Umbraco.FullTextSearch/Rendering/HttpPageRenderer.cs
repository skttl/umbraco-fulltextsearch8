using System;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Our.Umbraco.FullTextSearch.Interfaces;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace Our.Umbraco.FullTextSearch.Rendering;

/// <summary>
/// Renderer that performs a HTTP-request to get the content of a page.
/// </summary>
public class HttpPageRenderer : IPageRenderer
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<HttpPageRenderer> _logger;

    public HttpPageRenderer(
        IHttpClientFactory httpClientFactory, 
        ILogger<HttpPageRenderer> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public virtual string Render(IPublishedContent publishedContent, PublishedCultureInfo culture)
    {
        var publishedPageUrl = publishedContent.Url(mode: UrlMode.Absolute);

        try
        {
            // Using a named client to allow for configuration of default headers, cookies and more during service registration
            // if the named client is not registered during startup it will fallback so we never need to register if inside the package.

            var httpClient = _httpClientFactory.CreateClient(FullTextSearchConstants.HttpClientFactoryNamedClientName);
            var result = httpClient.GetAsync(publishedPageUrl).ConfigureAwait(false).GetAwaiter().GetResult();

            string fullHtml = string.Empty;
            
            // If the response is not status OK (like a 40X or 30X) we don't want to index the content.
            if (result.StatusCode == HttpStatusCode.OK)
            {
                fullHtml = result.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            }

            return fullHtml;

        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in http-request for full text indexing of page {nodeId}, tried to fetch {url}",publishedContent.Id, publishedPageUrl);
        }

        return string.Empty;
    }
}