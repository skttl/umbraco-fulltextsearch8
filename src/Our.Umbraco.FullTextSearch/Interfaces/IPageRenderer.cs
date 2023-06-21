using Umbraco.Cms.Core.Models.PublishedContent;

namespace Our.Umbraco.FullTextSearch.Interfaces;

/// <summary>
/// Abstraction for rendering page content based on the passed Umbraco content item.
/// </summary>
public interface IPageRenderer
{
    string Render(IPublishedContent publishedContent, PublishedCultureInfo culture);
}