using Our.Umbraco.FullTextSearch.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using Umbraco.Core.Logging;

namespace Our.Umbraco.FullTextSearch.Services
{
    public class Config : IConfig
    {
        private readonly ILogger _logger;

        public Config(ILogger logger)
        {
            _logger = logger;
        }

        private void Debug(string field, string value)
        {
            _logger.Debug<Config>("FullTextSearchValue of {field} is {value}", field, value);
        }

        public string GetDefaultTitleFieldName()
        {
            var value = ConfigurationManager.AppSettings["FullTextSearch.DefaultTitleFieldName"];
            Debug("FullTextSearch.FullTextFieldName", value);
            return string.IsNullOrWhiteSpace(value) ? "nodeName" : value;
        }

        public List<string> GetDisallowedContentTypeAliases()
        {
            var value = ConfigurationManager.AppSettings["FullTextSearch.DisallowedContentTypeAliases"];
            Debug("FullTextSearch.DisallowedContentTypeAliases", value);
            return string.IsNullOrWhiteSpace(value) ? new List<string>() : value.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
        }

        public List<string> GetDisallowedPropertyAliases()
        {
            var value = ConfigurationManager.AppSettings["FullTextSearch.DisallowedPropertyAliases"];
            Debug("FullTextSearch.DisallowedPropertyAliases", value);
            return string.IsNullOrWhiteSpace(value) ? new List<string>() : value.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
        }
        
        public string GetFullTextFieldName()
        {
            var value = ConfigurationManager.AppSettings["FullTextSearch.FullTextFieldName"];
            Debug("FullTextSearch.FullTextFieldName", value);
            return string.IsNullOrWhiteSpace(value) ? "FullTextSearch" : value;
        }

        public int GetHttpTimeout()
        {
            var timeout = 120;
            var value = ConfigurationManager.AppSettings["FullTextSearch.HttpTimeout"];
            Debug("FullTextSearch.HttpTimeout", value);
            int.TryParse(string.IsNullOrEmpty(value) ? timeout.ToString() : value, out timeout);
            return timeout;
        }
        public Uri GetHttpHost()
        {
            var value = ConfigurationManager.AppSettings["FullTextSearch.HttpHost"];
            Debug("FullTextSearch.HttpHost", value);
            return new Uri(string.IsNullOrWhiteSpace(value) ? "http://localhost/default.aspx" : value);
        }

        public string GetPathFieldName()
        {
            var value = ConfigurationManager.AppSettings["FullTextSearch.FullTextPathFieldName"];
            Debug("FullTextSearch.FullTextPathFieldName", value);
            return string.IsNullOrWhiteSpace(value) ? "FullTextPath" : value;
        }

        public string GetSearchActiveStringName()
        {
            var value = ConfigurationManager.AppSettings["FullTextSearch.SearchActiveStringName"];
            Debug("FullTextSearch.SearchActiveStringName", value);
            return string.IsNullOrWhiteSpace(value) ? "FullTextActive" : value;
        }

        public double GetSearchTitleBoost()
        {
            double titleBoost = 10.0;
            var value = ConfigurationManager.AppSettings["FullTextSearch.SearchTitleBoost"];
            Debug("FullTextSearch.SearchTitleBoost", value);
            double.TryParse(string.IsNullOrWhiteSpace(value) ? titleBoost.ToString(CultureInfo.InvariantCulture) : value, NumberStyles.Any, CultureInfo.InvariantCulture, out titleBoost);
            return titleBoost;
        }

        public List<string> GetXpathsToRemoveFromFullText()
        {
            var value = ConfigurationManager.AppSettings["FullTextSearch.XpathsToRemoveFromFullText"];
            Debug("FullTextSearch.XpathsToRemoveFromFullText", value);
            return string.IsNullOrWhiteSpace(value) ? new List<string>() : value.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
        }

        public bool IsFullTextIndexingEnabled()
        {
            var value = ConfigurationManager.AppSettings["FullTextSearch.Enabled"];
            Debug("FullTextSearch.Enabled", value);

            return !string.IsNullOrWhiteSpace(value) && (value == "1" || value.ToLower() == "true");
        }
    }
}
