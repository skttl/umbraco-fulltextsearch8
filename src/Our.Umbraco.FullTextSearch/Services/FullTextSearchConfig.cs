using Our.Umbraco.FullTextSearch.Interfaces;
using Our.Umbraco.FullTextSearch.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using Umbraco.Core;
using Umbraco.Core.Logging;

namespace Our.Umbraco.FullTextSearch.Services
{
    public class FullTextSearchConfig
    {
        private readonly ILogger _logger;
        private readonly string _configFilePath;

        private XmlDocument XmlDocument { get; set; }
        private XmlNode FullTextSearchNode { get; set; }

        public bool Enabled { get; internal set; }
        public string DefaultTitleField { get; internal set; }
        public string IndexingActiveKey { get; internal set; }
        public List<string> DisallowedContentTypeAliases { get; internal set; }
        public List<string> DisallowedPropertyAliases { get; internal set; }
        public List<string> XPathsToRemove { get; internal set; }
        public string FullTextContentField { get; internal set; }
        public string FullTextPathField { get; internal set; }


        public FullTextSearchConfig(ILogger logger)
        {
            _logger = logger;

            var appPath = HttpRuntime.AppDomainAppPath;
            _configFilePath = Path.Combine(appPath, ConfigurationManager.AppSettings["FullTextSearch.ConfigPath"] ?? @"App_Plugins\FullTextSearch\FullTextSearch.config");

            ResetConfigToDefaults();

            try
            {
                LoadXmlConfig();
                LoadConfig();
            }
            catch (Exception e)
            {
                _logger.Error<FullTextSearchConfig>(e, "Error parsing FullTextSearch.config.");
            }
        }

        public void ResetConfigToDefaults()
        {
            Enabled = true;
            DefaultTitleField = "nodeName";
            IndexingActiveKey = "FullTextIndexingActive";
            DisallowedContentTypeAliases = new List<string>();
            DisallowedPropertyAliases = new List<string>();
            XPathsToRemove = new List<string>();
            FullTextContentField = "FullTextContent";
            FullTextPathField = "FullTextPath";
        }

        private void LoadXmlConfig()
        {
            if (!File.Exists(_configFilePath))
            {
                _logger.Warn<FullTextSearchConfig>("Couldn't find config file {configFilePath}", _configFilePath);
            }
            else
            {
                XmlDocument = new XmlDocument();
                XmlDocument.Load(_configFilePath);
                FullTextSearchNode = XmlDocument.SelectSingleNode("FullTextSearch");
            }
        }

        public void LoadConfig()
        {
            if (FullTextSearchNode == null) return;

            Enabled = !(FullTextSearchNode.Attributes["enabled"]?.Value.InvariantEquals("false")).GetValueOrDefault();

            var indexingSection = FullTextSearchNode.SelectSingleNode("Indexing");
            if (indexingSection != null)
            {
                var defaultTitleField = indexingSection.SelectSingleNode("DefaultTitleField");
                if (defaultTitleField != null && !defaultTitleField.InnerText.IsNullOrWhiteSpace())
                    DefaultTitleField = defaultTitleField.InnerText;

                var indexingActiveKey = indexingSection.SelectSingleNode("IndexingActiveKey");
                if (indexingActiveKey != null && !indexingActiveKey.InnerText.IsNullOrWhiteSpace())
                    IndexingActiveKey = indexingActiveKey.InnerText;

                var disallowedContentTypeAliases = indexingSection.SelectNodes("DisallowedAliases/ContentTypes/add");
                if (disallowedContentTypeAliases != null)
                {
                    foreach (XmlNode disallowedContentTypeAlias in disallowedContentTypeAliases)
                    {
                        if (!disallowedContentTypeAlias.InnerText.IsNullOrWhiteSpace())
                            DisallowedContentTypeAliases.Add(disallowedContentTypeAlias.InnerText);
                    }
                }

                var disallowedPropertyAliases = indexingSection.SelectNodes("DisallowedAliases/Properties/add");
                if (disallowedPropertyAliases != null)
                {
                    foreach (XmlNode disallowedPropertyAlias in disallowedPropertyAliases)
                    {
                        if (!disallowedPropertyAlias.InnerText.IsNullOrWhiteSpace())
                            DisallowedPropertyAliases.Add(disallowedPropertyAlias.InnerText);
                    }
                }

                var xPathsToRemove = indexingSection.SelectNodes("XpathsToRemove/add");
                if (xPathsToRemove != null)
                {
                    foreach (XmlNode xPathToRemove in xPathsToRemove)
                    {
                        if (!xPathToRemove.InnerText.IsNullOrWhiteSpace())
                            XPathsToRemove.Add(xPathToRemove.InnerText);
                    }
                }

                var examineFieldNames = indexingSection.SelectSingleNode("ExamineFieldNames");
                if (examineFieldNames != null)
                {
                    var fullTextContent = examineFieldNames.SelectSingleNode("FullTextContent");
                    if (fullTextContent != null && !fullTextContent.InnerText.IsNullOrWhiteSpace())
                        FullTextContentField = fullTextContent.InnerText;

                    var fullTextPath = examineFieldNames.SelectSingleNode("FullTextPath");
                    if (fullTextPath != null && !fullTextPath.InnerText.IsNullOrWhiteSpace())
                        FullTextPathField = fullTextPath.InnerText;
                }

            }
        }

    }
}
