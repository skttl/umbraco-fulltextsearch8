using Examine;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Our.Umbraco.FullTextSearch.Interfaces;
using Our.Umbraco.FullTextSearch.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Infrastructure.Examine;
using Umbraco.Extensions;

namespace Our.Umbraco.FullTextSearch.NotificationHandlers
{
    public class AddFullTextItemsToIndex : INotificationHandler<UmbracoApplicationStartingNotification>
    {
        private readonly IExamineManager _examineManager;
        private readonly FullTextSearchOptions _options;
        private readonly ILogger<AddFullTextItemsToIndex> _logger;
        private readonly IProfilingLogger _profilingLogger;
        private readonly ICacheService _cacheService;

        public AddFullTextItemsToIndex(IExamineManager examineManager,
            IOptions<FullTextSearchOptions> options,
            ILogger<AddFullTextItemsToIndex> logger,
            IProfilingLogger profilingLogger,
            ICacheService cacheService)
        {
            _examineManager = examineManager;
            _options = options.Value;
            _logger = logger;
            _profilingLogger = profilingLogger;
            _cacheService = cacheService;
        }

        public void Handle(UmbracoApplicationStartingNotification notification)
        {
            if (!_examineManager.TryGetIndex(Constants.UmbracoIndexes.ExternalIndexName, out IIndex index))
            {
                _logger.LogError(new InvalidOperationException($"{Constants.UmbracoIndexes.ExternalIndexName} not found"), $"{Constants.UmbracoIndexes.ExternalIndexName} not found");
                return;
            }

            //we need to cast because BaseIndexProvider contains the TransformingIndexValues event
            if (index is not BaseIndexProvider indexProvider)
            {
                _logger.LogError(new InvalidOperationException($"Could not cast {Constants.UmbracoIndexes.ExternalIndexName} to BaseIndexProvider"), $"Could not cast {Constants.UmbracoIndexes.ExternalIndexName} to BaseIndexProvider");
                return;
            }

            _logger.LogDebug("Attaching TransformingIndexValues-handler for Full Text Search");

            indexProvider.TransformingIndexValues += IndexProviderTransformingIndexValues;
        }

        private void IndexProviderTransformingIndexValues(object sender, IndexingItemEventArgs e)
        {
            _logger.LogDebug("Handling TransformingIndexValues for {NodeId}", e.ValueSet.Id);

            if (e.Index.Name != Constants.UmbracoIndexes.ExternalIndexName) return;
            if (e.ValueSet.Category != IndexTypes.Content) return;
            if (!_options.Enabled)
            {
                _logger.LogDebug("FullTextIndexing is not enabled");
                return;
            }

            // check if contentType is allowed
            if (_options.DisallowedContentTypeAliases.InvariantContains(e.ValueSet.ItemType))
            {
                _logger.LogDebug(
                    "{nodeTypeAlias} is disallowed by DisallowedContentTypeAliases - {disallowedContentTypeAliases}",
                    e.ValueSet.ItemType,
                    string.Join(",", _options.DisallowedContentTypeAliases)
                    );
                return;
            }

            if (_options.DisallowedPropertyAliases.Any())
            {
                foreach (var disallowedPropertyAlias in _options.DisallowedPropertyAliases)
                {
                    var value = e.ValueSet.GetValue(disallowedPropertyAlias);
                    if (value != null && value.ToString() == "1")
                    {
                        _logger.LogDebug("Node {nodeId} was excluded since to disallowed property alias {disallowedPropertyAlias} was equal to 1.", e.ValueSet.Id, disallowedPropertyAlias);
                        return;
                    }
                }
            }

            // check if there is a template
            var templateId = e.ValueSet.GetValue("templateID");
            if (templateId == null || templateId.ToString() == "0")
            {
                _logger.LogDebug("Template Id is 0 or null");
                return;
            }

            var updatedValues = e.ValueSet.Values.ToDictionary(x => x.Key, x => (IEnumerable<object>)x.Value);

            // set path value
            var currentPath = e.ValueSet.GetValue("path");
            if (currentPath != null)
            {
                var pathFieldName = _options.FullTextPathField;
                updatedValues[pathFieldName] = new List<object> { currentPath.ToString().Replace(",", " ") };
            }

            // convert id to int, so we can get it from the content cache
            if (!int.TryParse(e.ValueSet.Id, out int id))
            {
                _logger.LogWarning("Exit indexing since node id was {NodeId}",e.ValueSet.Id);

                return;
            }


            using (_profilingLogger.DebugDuration<AddFullTextItemsToIndex>("Attempt to fulltext index for node " + id, "Completed fulltext index for node " + id))
            {
                
                var cacheItems = _cacheService.GetFromCache(id).ConfigureAwait(false).GetAwaiter().GetResult();
                if (cacheItems == null || !cacheItems.Any())
                {
                    _logger.LogDebug("Exit TransformIndexValues since no cached item for node {NodeId} was found.", id);
                    return;
                }

                foreach (var item in cacheItems)
                {
                    var fieldName = _options.FullTextContentField;
                    if (item.Culture != "") fieldName += "_" + item.Culture;

                    _logger.LogDebug("Setting field {FieldName} to {FieldValue}",fieldName, item.Text);

                    updatedValues[fieldName] = new List<object> { item.Text };
                    
                }
            }

            e.SetValues(updatedValues);

            _logger.LogDebug("Transformed {UpdatedPropertiesCount} index values for node {NodeId}", updatedValues.Count, e.ValueSet.Id);
        }
    }
}
