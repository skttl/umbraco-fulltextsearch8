using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Our.Umbraco.FullTextSearch.Interfaces;
using Our.Umbraco.FullTextSearch.Options;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace Our.Umbraco.FullTextSearch.Rendering;

/// <summary>
/// Renderer that performs a HTTP-request to get the content of a page.
/// </summary>
public class HttpPageRenderer : IPageRenderer
{
    private readonly FullTextSearchOptions _options;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<HttpPageRenderer> _logger;

    public HttpPageRenderer(
        IOptions<FullTextSearchOptions> options,
        IHttpClientFactory httpClientFactory,
        ILogger<HttpPageRenderer> logger)
    {
        _options = options.Value;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public virtual async Task<string> Render(IPublishedContent publishedContent, PublishedCultureInfo culture)
    {
        var publishedPageUrl = publishedContent.Url(mode: UrlMode.Absolute);

        if (!Uri.TryCreate(publishedPageUrl, UriKind.Absolute, out var url))
        {
            _logger.LogInformation("Unable to render page {NodeId}, url returned is invalid ({Url})", publishedContent.Id, publishedPageUrl);
            return string.Empty;
        }

        try
        {
            // Using a named client to allow for configuration of default headers, cookies and more during service registration
            // if the named client is not registered during startup it will fallback so we never need to register if inside the package.

            var httpClient = _httpClientFactory.CreateClient(FullTextSearchConstants.HttpClientFactoryNamedClientName);
            httpClient.DefaultRequestHeaders.Add(FullTextSearchConstants.HttpClientRequestHeaderName, _options.RenderingActiveKey);

            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("FullTextSearch: Processing node {NodeId}, fetching {Url}", publishedContent.Id, publishedPageUrl);

            var result = await httpClient.GetAsync(publishedPageUrl);

            string fullHtml = string.Empty;

            // If the response is not status OK (like a 40X or 30X) we don't want to index the content.
            if (result.StatusCode == HttpStatusCode.OK)
            {
                fullHtml = await result.Content.ReadAsStringAsync();
            }
            else if(_logger.IsEnabled(LogLevel.Debug))
            {
                string pageContent = await result.Content.ReadAsStringAsync();
                _logger.LogDebug("FullTextSearch: Status {HttpStatus} when rendering node {NodeId}. Content: {PageContent}", result.StatusCode, publishedContent.Id, pageContent);
            }

            return fullHtml;

        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in http-request for full text indexing of page {NodeId}, tried to fetch {Url}", publishedContent.Id, publishedPageUrl);
        }

        return string.Empty;
    }
}