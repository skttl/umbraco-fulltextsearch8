using Examine;
using Examine.Providers;
using Our.Umbraco.FullTextSearch.Interfaces;
using System;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Logging;
using Umbraco.Examine;
using Umbraco.Web.Search;

namespace Our.Umbraco.FullTextSearch.Components
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    [ComposeAfter(typeof(ExamineComposer))]
    public class AddFullTextItemsToIndexComposer : ComponentComposer<AddFullTextItemsToIndex> { }
    public class AddFullTextItemsToIndex : IComponent
    {
        private readonly IExamineManager _examineManager;
        private readonly IConfig _fullTextConfig;
        private readonly ILogger _logger;
        private readonly IProfilingLogger _profilingLogger;
        private readonly ICacheService _cacheService;

        public AddFullTextItemsToIndex(IExamineManager examineManager,
            IConfig fullTextConfig,
            ILogger logger,
            IProfilingLogger profilingLogger,
            ICacheService cacheService)
        {
            _examineManager = examineManager;
            _fullTextConfig = fullTextConfig;
            _logger = logger;
            _profilingLogger = profilingLogger;
            _cacheService = cacheService;
        }

        public void Initialize()
        {
            if (!_examineManager.TryGetIndex("ExternalIndex", out IIndex index))
            {
                _logger.Error<AddFullTextItemsToIndex>(new InvalidOperationException("No index found by name ExternalIndex"));
                return;
            }

            //we need to cast because BaseIndexProvider contains the TransformingIndexValues event
            if (!(index is BaseIndexProvider indexProvider))
            {
                _logger.Error<AddFullTextItemsToIndex>(new InvalidOperationException("Could not cast ExternalIndex to BaseIndexProvider"));
                return;
            }
            indexProvider.TransformingIndexValues += IndexProviderTransformingIndexValues;
        }

        private void IndexProviderTransformingIndexValues(object sender, IndexingItemEventArgs e)
        {
            if (e.Index.Name != "ExternalIndex") return;
            if (e.ValueSet.Category != IndexTypes.Content) return;
            if (!_fullTextConfig.IsFullTextIndexingEnabled())
            {
                _logger.Debug<AddFullTextItemsToIndex>("FullTextIndexing is not enabled");
                return;
            }

            // check if contentType is allowed
            var nodeTypeAlias = e.ValueSet.GetValue("__NodeTypeAlias");
            if (nodeTypeAlias != null && _fullTextConfig.GetDisallowedContentTypeAliases().Contains(nodeTypeAlias.ToString()))
            {
                _logger.Debug<AddFullTextItemsToIndex>("{nodeTypeAlias} is disallowed by DisallowedContentTypeAliases - {disallowedContentTypeAliases}", nodeTypeAlias.ToString(), string.Join(",", _fullTextConfig.GetDisallowedContentTypeAliases()));
                return;
            }

            if (_fullTextConfig.GetDisallowedPropertyAliases().Any())
            {
                foreach (var disallowedPropertyAlias in _fullTextConfig.GetDisallowedPropertyAliases())
                {
                    var value = e.ValueSet.GetValue(disallowedPropertyAlias);
                    if (value != null && value.ToString() == "1")
                    {
                        return;
                    }
                }
            }

            // check if there is a template
            var templateId = e.ValueSet.GetValue("templateID");
            if (templateId == null || templateId.ToString() == "0")
            {
                _logger.Debug<AddFullTextItemsToIndex>("Template Id is 0 or null");
                return;
            }

            // set path value
            var currentPath = e.ValueSet.GetValue("path");
            if (currentPath != null)
            {
                var pathFieldName = _fullTextConfig.GetPathFieldName();
                e.ValueSet.TryAdd(pathFieldName, currentPath.ToString().Replace(",", " "));
            }

            // convert id to int, so we can get it from the content cache
            if (!int.TryParse(e.ValueSet.Id, out int id)) return;

            using (_profilingLogger.DebugDuration<AddFullTextItemsToIndex>("Attempt to fulltext index for node " + id, "Completed fulltext index for node " + id))
            {
                var cacheItems = _cacheService.GetFromCache(id);
                if (cacheItems == null || !cacheItems.Any()) return;

                foreach (var item in cacheItems)
                {
                    var fieldName = _fullTextConfig.GetFullTextFieldName();
                    if (item.Culture != "") fieldName += "_" + item.Culture;

                    e.ValueSet.TryAdd(fieldName, item.Text);
                }
            }
        }

        public void Terminate()
        {
        }
    }
}
