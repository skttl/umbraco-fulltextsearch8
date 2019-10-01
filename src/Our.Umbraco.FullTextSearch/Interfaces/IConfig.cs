using System;
using System.Collections.Generic;

namespace Our.Umbraco.FullTextSearch.Interfaces
{
    public interface IConfig
    {
        string GetDefaultTitleFieldName();
        List<string> GetDisallowedContentTypeAliases();
        List<string> GetDisallowedPropertyAliases();
        string GetFullTextFieldName();
        int GetHttpTimeout();
        string GetPathFieldName();
        string GetSearchActiveStringName();
        double GetSearchTitleBoost();
        List<string> GetXpathsToRemoveFromFullText();
        bool IsFullTextIndexingEnabled();
    }
}
