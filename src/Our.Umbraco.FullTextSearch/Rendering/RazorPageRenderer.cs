using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Our.Umbraco.FullTextSearch.Interfaces;
using Our.Umbraco.FullTextSearch.Options;
using System.Threading.Tasks;
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
    private readonly IUmbracoComponentRenderer _umbracoComponentRenderer;
    private readonly IVariationContextAccessor _variationContextAccessor;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private FullTextSearchOptions _options;

    public RazorPageRenderer(

        IUmbracoComponentRenderer umbracoComponentRenderer,
        IVariationContextAccessor variationContextAccessor,
        IHttpContextAccessor httpContextAccessor,
        IOptions<FullTextSearchOptions> options)
    {
        _umbracoComponentRenderer = umbracoComponentRenderer;
        _variationContextAccessor = variationContextAccessor;
        _httpContextAccessor = httpContextAccessor;
        _options = options.Value;
    }

    public virtual async Task<string> Render(IPublishedContent publishedContent, PublishedCultureInfo culture)
    {
        if (!culture.Culture.IsNullOrWhiteSpace())
            _variationContextAccessor.VariationContext = new VariationContext(culture.Culture);

        _httpContextAccessor.HttpContext?.Request.Headers.Append(FullTextSearchConstants.HttpClientRequestHeaderName, _options.RenderingActiveKey);

        // todo do we need the wrapping template?
        var fullHtml = await _umbracoComponentRenderer.RenderTemplateAsync(publishedContent.Id, publishedContent.TemplateId);

        _httpContextAccessor.HttpContext?.Request.Headers.Remove(FullTextSearchConstants.HttpClientRequestHeaderName);

        return fullHtml.ToString();
    }
}