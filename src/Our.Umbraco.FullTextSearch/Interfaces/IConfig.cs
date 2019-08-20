using System.Collections.Generic;

namespace Our.Umbraco.FullTextSearch.Interfaces
{
    public interface IConfig
    {
        bool GetBooleanByKey(string key);
        string GetByKey(string key);
        double? GetDoubleByKey(string key);
        List<string> GetMultiByKey(string key);
        string GetFullTextFieldName();
        string GetPathFieldName();
        List<string> GetDisallowedContentTypeAliases();
        List<string> GetDisallowedPropertyAliases();
        double GetSearchTitleBoost();
    }
}
