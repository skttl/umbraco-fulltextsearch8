using Newtonsoft.Json;
using System.Collections.Generic;

namespace Our.Umbraco.FullTextSearch.Options
{
    public class FullTextSearchOptions
    {
        [JsonProperty("enabled")]
        public bool Enabled { get; set; } = true;
        [JsonProperty("defaultTitleField")]
        public string DefaultTitleField { get; set; } = "nodeName";
        [JsonProperty("indexingActiveKey")]
        public string IndexingActiveKey { get; set; } = "FullTextIndexingActive";
        [JsonProperty("disallowedContentTypeAliases")]
        public List<string> DisallowedContentTypeAliases { get; set; } = new List<string>();
        [JsonProperty("disallowedPropertyAliases")]
        public List<string> DisallowedPropertyAliases { get; set; } = new List<string>();
        [JsonProperty("xPathsToRemove")]
        public List<string> XPathsToRemove { get; set; } = new List<string>();
        [JsonProperty("fullTextContentField")]
        public string FullTextContentField { get; set; } = "FullTextContent";
        [JsonProperty("fullTextPathField")]
        public string FullTextPathField { get; set; } = "FullTextPath";
    }
}
