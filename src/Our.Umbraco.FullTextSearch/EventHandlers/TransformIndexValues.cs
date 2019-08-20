using Examine;
using Examine.Providers;
using Our.Umbraco.FullTextSearch.Interfaces;
using System;
using System.Linq;
using Umbraco.Core.Composing;
using Umbraco.Core.Logging;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Examine;
using Umbraco.Web;

namespace Our.Umbraco.FullTextSearch.EventHandlers
{
    public class TransformIndexValues : IComponent
    {
        private readonly IExamineManager _examineManager;
        private readonly IConfig _fullTextConfig;
        private readonly IHtmlService _htmlService;
        private readonly ILogger _logger;
        private readonly IUmbracoContextFactory _umbracoContextFactory;
        private readonly IProfilingLogger _profilingLogger;

        public TransformIndexValues(IExamineManager examineManager,
            IConfig fullTextConfig,
            IHtmlService htmlService,
            ILogger logger,
            IProfilingLogger profilingLogger,
            IUmbracoContextFactory umbracoContextFactory)
        {
            _examineManager = examineManager;
            _fullTextConfig = fullTextConfig;
            _htmlService = htmlService;
            _logger = logger;
            _umbracoContextFactory = umbracoContextFactory;
            _profilingLogger = profilingLogger;
        }

        public void Initialize()
        {
            if (!_examineManager.TryGetIndex("ExternalIndex", out IIndex index))
            {
                _logger.Error<TransformIndexValues>(new InvalidOperationException("No index found by name ExternalIndex"));
                return;
            }

            //we need to cast because BaseIndexProvider contains the TransformingIndexValues event
            if (!(index is BaseIndexProvider indexProvider))
            {
                _logger.Error<TransformIndexValues>(new InvalidOperationException("Could not cast ExternalIndex to BaseIndexProvider"));
                return;
            }

            indexProvider.TransformingIndexValues += IndexProviderTransformingIndexValues;
        }

        private void IndexProviderTransformingIndexValues(object sender, IndexingItemEventArgs e)
        {
            if (e.Index.Name != "ExternalIndex") return;
            if (e.ValueSet.Category != IndexTypes.Content) return;
            if (!_fullTextConfig.GetBooleanByKey("Enabled")) return;

            // check if contentType is allowed
            var nodeTypeAlias = e.ValueSet.GetValue("__NodeTypeAlias");
            if (nodeTypeAlias != null && _fullTextConfig.GetDisallowedContentTypeAliases().Contains(nodeTypeAlias.ToString())) return;

            // check if there is a template
            var templateId = e.ValueSet.GetValue("templateID");
            if (templateId == null || templateId.ToString() == "0") return;

            // convert id to int, so we can get it from the content cache
            if (!int.TryParse(e.ValueSet.Id, out int id)) return;

            using (_profilingLogger.DebugDuration<TransformIndexValues>("Attempt to fulltext index for node " + id, "Completed fulltext index for node " + id))
            {
                using (var cref = _umbracoContextFactory.EnsureUmbracoContext())
                {
                    var content = cref.UmbracoContext.Content.GetById(id);
                    if (content == null) return;

                    // check if indexing is disallowed by config/property
                    if (_fullTextConfig.GetDisallowedPropertyAliases().Any(x => content.Value<bool>(x))) return;

                    // get content of page, and manipulate for indexing
                    var url = content.Url(null, UrlMode.Absolute);
                    _htmlService.GetHtmlByUrl(url, out string fullHtml);
                    var fullText = _htmlService.GetTextFromHtml(fullHtml);
                    var success = e.ValueSet.TryAdd(_fullTextConfig.GetFullTextFieldName(), fullText);

                    // loop through cultures, if there is more than one
                    if (content.Cultures.Count > 1)
                    {
                        foreach (var culture in content.Cultures)
                        {
                            // get content of page, and manipulate for indexing
                            url = content.Url(culture.Value.Culture, UrlMode.Absolute);
                            _htmlService.GetHtmlByUrl(url, out fullHtml);
                            fullText = _htmlService.GetTextFromHtml(fullHtml);
                            success = e.ValueSet.TryAdd(_fullTextConfig.GetFullTextFieldName() + "_" + culture.Value.Culture, fullText);
                        }
                    }
                }
            }
        }

        public void Terminate()
        {
        }
    }
}
