using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Our.Umbraco.FullTextSearch.Interfaces;
using Our.Umbraco.FullTextSearch.Options;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Templates;
using Umbraco.Extensions;

namespace Our.Umbraco.FullTextSearch.Rendering;

/// <summary>
/// Renderer that uses the Umbraco Component Renderer to render HTML content from the razor views.
/// </summary>
/// <remarks>
/// The <see cref="IUmbracoComponentRenderer"/> has known limitation for scenarios when route hijacking is used with custom view models.
/// In these cases you would want to use the <see cref="HttpPageRenderer"/>
/// </remarks>
public class RazorPageRenderer : IPageRenderer
{
    private readonly IProfilingLogger _profilingLogger;
    private readonly IUmbracoComponentRenderer _umbracoComponentRenderer;
    private readonly IVariationContextAccessor _variationContextAccessor;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private FullTextSearchOptions _options;

    public RazorPageRenderer(
        IProfilingLogger profilingLogger,
        IUmbracoComponentRenderer umbracoComponentRenderer,
        IVariationContextAccessor variationContextAccessor,
        IHttpContextAccessor httpContextAccessor,
        IOptions<FullTextSearchOptions> options)
    {
        _profilingLogger = profilingLogger;
        _umbracoComponentRenderer = umbracoComponentRenderer;
        _variationContextAccessor = variationContextAccessor;
        _httpContextAccessor = httpContextAccessor;
        _options = options.Value;
    }

    public virtual string Render(IPublishedContent publishedContent, PublishedCultureInfo culture)
    {
        using (_profilingLogger.DebugDuration<RazorPageRenderer>($"Rendering {publishedContent.Id} {culture.Culture} {publishedContent.Name}", $"Completed rendering {publishedContent.Id} {culture.Culture} {publishedContent.Name}"))
        {
            if (!culture.Culture.IsNullOrWhiteSpace())
                _variationContextAccessor.VariationContext = new VariationContext(culture.Culture);

            var fullHtmlString = string.Empty;
            if (_httpContextAccessor.HttpContext != null)
            {
                var currentUserAgent = _httpContextAccessor.HttpContext.Request.Headers.UserAgent;
                _httpContextAccessor.HttpContext.Request.Headers.UserAgent = FullTextSearchConstants.HttpClientFactoryNamedClientName;

                var fullHtml = _umbracoComponentRenderer.RenderTemplateAsync(publishedContent.Id, publishedContent.TemplateId).ConfigureAwait(false).GetAwaiter().GetResult();
                fullHtmlString = fullHtml.ToString();

                _httpContextAccessor.HttpContext.Request.Headers.UserAgent = currentUserAgent;
            }
            return fullHtmlString;
        }
    }
}