using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Our.Umbraco.FullTextSearch.Options
{
    public class FullTextSearchOptions
    {
        public bool Enabled { get; set; } = true;
        public string DefaultTitleField { get; set; } = "nodeName";
        public string IndexingActiveKey { get; set; } = "FullTextIndexingActive";
        public List<string> DisallowedContentTypeAliases { get; set; } = new List<string>();
        public List<string> DisallowedPropertyAliases { get; set; } = new List<string>();
        public List<string> XPathsToRemove { get; set; } = new List<string>();
        public string FullTextContentField { get; set; } = "FullTextContent";
        public string FullTextPathField { get; set; } = "FullTextPath";
    }
}
