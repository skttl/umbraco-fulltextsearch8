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
            _configFilePath = Path.Combine(appPath, ConfigurationManager.AppSettings["FullTextSearch.ConfigPath"] ?? @"App_Plugins\FullTextSearch.config");

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
            XmlDocument = new XmlDocument();
            if (!File.Exists(_configFilePath))
            {
                _logger.Warn<FullTextSearchConfig>("Couldn't find config file {configFilePath}, creating one instead.", _configFilePath);
                XmlDocument.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8"" ?><FullTextSearch></FullTextSearch>");
            }
            else
            {
                XmlDocument.Load(_configFilePath);
            }

            FullTextSearchNode = GetOrCreateSingleNode(XmlDocument, "FullTextSearch");
        }

        private bool SaveXmlConfig()
        {
            try
            {
                XmlDocument.Save(_configFilePath);
                LoadConfig();
                return true;
            }
            catch (Exception e)
            {
                _logger.Error<FullTextSearchConfig>(e, "Error saving FullTextSearch.config.");
                return false;
            }
        }

        public void LoadConfig()
        {
            Enabled = !(FullTextSearchNode.Attributes["enabled"]?.Value.InvariantEquals("false")).GetValueOrDefault();

            var indexingSection = FullTextSearchNode.SelectSingleNode("/Indexing");
            if (indexingSection != null)
            {
                var defaultTitleField = indexingSection.SelectSingleNode("/DefaultTitleField");
                if (defaultTitleField != null && !defaultTitleField.InnerText.IsNullOrWhiteSpace())
                    DefaultTitleField = defaultTitleField.InnerText;

                var indexingActiveKey = indexingSection.SelectSingleNode("/IndexingActiveKey");
                if (indexingActiveKey != null && !indexingActiveKey.InnerText.IsNullOrWhiteSpace())
                    IndexingActiveKey = indexingActiveKey.InnerText;

                var disallowedContentTypeAliases = indexingSection.SelectNodes("/DisallowedAliases/ContentTypes/add");
                if (disallowedContentTypeAliases != null)
                {
                    foreach (XmlNode disallowedContentTypeAlias in disallowedContentTypeAliases)
                    {
                        if (!disallowedContentTypeAlias.InnerText.IsNullOrWhiteSpace())
                            DisallowedContentTypeAliases.Add(disallowedContentTypeAlias.InnerText);
                    }
                }

                var disallowedPropertyAliases = indexingSection.SelectNodes("/DisallowedAliases/Properties/add");
                if (disallowedPropertyAliases != null)
                {
                    foreach (XmlNode disallowedPropertyAlias in disallowedPropertyAliases)
                    {
                        if (!disallowedPropertyAlias.InnerText.IsNullOrWhiteSpace())
                            DisallowedPropertyAliases.Add(disallowedPropertyAlias.InnerText);
                    }
                }

                var xPathsToRemove = indexingSection.SelectNodes("/XpathsToRemove/add");
                if (xPathsToRemove != null)
                {
                    foreach (XmlNode xPathToRemove in xPathsToRemove)
                    {
                        if (!xPathToRemove.InnerText.IsNullOrWhiteSpace())
                            XPathsToRemove.Add(xPathToRemove.InnerText);
                    }
                }

                var examineFieldNames = indexingSection.SelectSingleNode("/ExamineFieldNames");
                if (examineFieldNames != null)
                {
                    var fullTextContent = examineFieldNames.SelectSingleNode("/FullTextContent");
                    if (fullTextContent != null && !fullTextContent.InnerText.IsNullOrWhiteSpace())
                        FullTextContentField = fullTextContent.InnerText;

                    var fullTextPath = examineFieldNames.SelectSingleNode("/FullTextPath");
                    if (fullTextPath != null && !fullTextPath.InnerText.IsNullOrWhiteSpace())
                        FullTextPathField = fullTextPath.InnerText;
                }

            }
        }

        private XmlNode CreateSingleNode(XmlNode parent, string nodeName)
        {
            var newNode = XmlDocument.CreateElement(nodeName);
            parent.AppendChild(newNode);
            return newNode;
        }

        private XmlNode GetOrCreateSingleNode(XmlNode parent, string nodeName)
        {
            return parent.SelectSingleNode("/" + nodeName) ?? CreateSingleNode(parent, nodeName);
        }

        public void SetDefaultTitleField(string value)
        {
            var indexing = GetOrCreateSingleNode(FullTextSearchNode, "Indexing");
            var node = GetOrCreateSingleNode(indexing, "DefaultTitleField");
            node.InnerText = value;
        }

        public void SetIndexingActiveKey(string value)
        {
            var indexing = GetOrCreateSingleNode(FullTextSearchNode, "Indexing");
            var node = GetOrCreateSingleNode(indexing, "IndexingActiveKey");
            node.InnerText = value;
        }

        public void AddDisallowedContentType(string value)
        {
            if (DisallowedContentTypeAliases.Contains(value)) return;

            DisallowedContentTypeAliases.Add(value);
        }

        public void RemoveDisallowedContentType(string value)
        {
            if (!DisallowedContentTypeAliases.Contains(value)) return;

            DisallowedContentTypeAliases.Remove(value);
        }

        public void SetDisallowedContentTypes(List<string> values)
        {
            var indexing = GetOrCreateSingleNode(FullTextSearchNode, "Indexing");
            var disallowedAliases = GetOrCreateSingleNode(indexing, "DisallowedAliases");
            var contentTypes = GetOrCreateSingleNode(disallowedAliases, "ContentTypes");

            while (contentTypes.HasChildNodes)
                contentTypes.RemoveChild(contentTypes.FirstChild);

            foreach (var value in values)
            {
                if (value.IsNullOrWhiteSpace()) continue;
                var newValue = XmlDocument.CreateElement("add");
                newValue.InnerText = value;
                contentTypes.AppendChild(newValue);
            }
        }

        public void AddDisallowedProperty(string value)
        {
            if (DisallowedPropertyAliases.Contains(value)) return;

            DisallowedPropertyAliases.Add(value);
        }

        public void RemoveDisallowedProperty(string value)
        {
            if (!DisallowedPropertyAliases.Contains(value)) return;

            DisallowedPropertyAliases.Remove(value);
        }

        public void SetDisallowedProperties(List<string> values)
        {
            var indexing = GetOrCreateSingleNode(FullTextSearchNode, "Indexing");
            var disallowedAliases = GetOrCreateSingleNode(indexing, "DisallowedAliases");
            var properties = GetOrCreateSingleNode(disallowedAliases, "Properties");

            while (properties.HasChildNodes)
                properties.RemoveChild(properties.FirstChild);

            foreach (var value in values)
            {
                if (value.IsNullOrWhiteSpace()) continue;
                var newValue = XmlDocument.CreateElement("add");
                newValue.InnerText = value;
                properties.AppendChild(newValue);
            }
        }

        public void AddXPathToRemove(string value)
        {
            if (XPathsToRemove.Contains(value)) return;

            XPathsToRemove.Add(value);
        }

        public void RemoveXPathToRemove(string value)
        {
            if (!XPathsToRemove.Contains(value)) return;

            XPathsToRemove.Remove(value);
        }

        public void SetXPathsToRemove(List<string> values)
        {
            var indexing = GetOrCreateSingleNode(FullTextSearchNode, "Indexing");
            var xPathsToRemove = GetOrCreateSingleNode(indexing, "XPathsToRemove");

            while (xPathsToRemove.HasChildNodes)
                xPathsToRemove.RemoveChild(xPathsToRemove.FirstChild);

            foreach (var value in values)
            {
                if (value.IsNullOrWhiteSpace()) continue;
                var newValue = XmlDocument.CreateElement("add");
                newValue.InnerText = value;
                xPathsToRemove.AppendChild(newValue);
            }
        }

        public void SetFullTextContentField(string value)
        {
            var indexing = GetOrCreateSingleNode(FullTextSearchNode, "Indexing");
            var fieldNames = GetOrCreateSingleNode(indexing, "ExamineFieldNames");
            var node = GetOrCreateSingleNode(fieldNames, "FullTextContent");
            node.InnerText = value;
        }

        public void SetFullTextPathField(string value)
        {
            var indexing = GetOrCreateSingleNode(FullTextSearchNode, "Indexing");
            var fieldNames = GetOrCreateSingleNode(indexing, "ExamineFieldNames");
            var node = GetOrCreateSingleNode(fieldNames, "FullTextPath");
            node.InnerText = value;
        }

    }
}
