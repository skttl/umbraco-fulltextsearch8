using System.Collections.Generic;

namespace Our.Umbraco.FullTextSearch.Interfaces
{
    public interface IFullTextSearchConfig
    {
        string DefaultTitleField { get; }
        List<string> DisallowedContentTypeAliases { get; }
        List<string> DisallowedPropertyAliases { get; }
        bool Enabled { get; }
        string FullTextContentField { get; }
        string FullTextPathField { get; }
        string IndexingActiveKey { get; }
        List<string> XPathsToRemove { get; }

        void LoadConfig();
        void ResetConfigToDefaults();
    }
}