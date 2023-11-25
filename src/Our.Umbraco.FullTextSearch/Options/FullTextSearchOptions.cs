using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Our.Umbraco.FullTextSearch.Options;

public class FullTextSearchOptions
{
    /// <summary>
    /// Indexing the full text content is by default enabled, but you can disable it by setting `Enabled` to false.
    /// </summary>
    [JsonProperty("enabled")]
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// The `DefaultTitleField` node contains the name of the field containing the title of the page in the index. The default value is `nodeName`. You can also override this when searching.
    /// </summary>
    [JsonProperty("defaultTitleField")]
    public string DefaultTitleField { get; set; } = "nodeName";

    [Obsolete("Use RenderingActiveKey instead")]
    [JsonProperty("indexingActiveKey")]
    public string IndexingActiveKey { get; set; } = "FullTextRenderingActive";

    /// <summary>
    /// When rendering, FullTextSearch adds the value of `RenderingActiveKey` as the value of a Request header named `X-Umbraco-FullTextSearch`, so you can use that to send different content to the renderer. The default value is FullTextRenderingActive. You can also use the `IsRenderingActive` helper method, in your views, to determine whether or not FullTextSearch is rendering the page. You can use this to exclude parts of the views from the content being rendered/indexed.
    /// </summary>
    [JsonProperty("renderingActiveKey")]
    public string RenderingActiveKey { get; set; } = "FullTextRenderingActive";

    /// <summary>
    /// By default, all nodes with a template will be cached and indexed. You can control which nodes are being indexed, by adding the aliases of the disallowed content type aliases here.
    /// </summary>
    [JsonProperty("disallowedContentTypeAliases")]
    public List<string> DisallowedContentTypeAliases { get; set; } = new List<string>();


    /// <summary>
    /// By default, all nodes with a template will be cached and indexed. You can control which nodes are being indexed, by adding the aliases of the properties containing a true/false editor, to control whether or not to include a node. A true value means it will be disallowed.
    /// </summary>
    [JsonProperty("disallowedPropertyAliases")]
    public List<string> DisallowedPropertyAliases { get; set; } = new List<string>();

    /// <summary>
    /// Add specific Xpaths to remove from the rendered content. Using this, you can ie. remove scripts (`//script`) or the head area ('//head') of the page.
    /// </summary>
    [JsonProperty("xPathsToRemove")]
    public List<string> XPathsToRemove { get; set; } = new List<string>();

    /// <summary>
    /// Field name to use in ExternalIndex for rendered full text content of pages.
    /// </summary>
    [JsonProperty("fullTextContentField")]
    public string FullTextContentField { get; set; } = "FullTextContent";

    /// <summary>
    /// Field name to use for Full Text Searchs path field, used for determining hierarchy between pages.
    /// </summary>
    [JsonProperty("fullTextPathField")]
    public string FullTextPathField { get; set; } = "FullTextPath";

    /// <summary>
    /// Pattern to use for highlighting text in search result html. Example: &lt;b&gt;{0}&lt;/b&gt;
    /// </summary>
    [JsonProperty("highlightPattern")]
    public string HighlightPattern { get; set; } = "<b>{0}</b>";
}
