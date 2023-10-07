using Examine;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Our.Umbraco.FullTextSearch.Interfaces;
using Our.Umbraco.FullTextSearch.Options;
using System.Linq;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Examine;
using Umbraco.Cms.Infrastructure.Migrations;

namespace Our.Umbraco.FullTextSearch.Migrations.FourZeroZero;

public class ReindexEverything : MigrationBase
{
    private readonly IUmbracoContextFactory _umbracoContextFactory;
    private readonly IIndexRebuilder _indexRebuilder;
    private readonly ICacheService _cacheService;
    private readonly FullTextSearchOptions _options;
    private readonly IExamineManager _examineManager;
    private readonly ILogger<ReindexEverything> _logger;

    public ReindexEverything(IMigrationContext context, IIndexRebuilder indexRebuilder, ICacheService cacheService, IOptions<FullTextSearchOptions> options, IExamineManager examineManager, ILogger<ReindexEverything> logger, IUmbracoContextFactory umbracoContextFactory) : base(context)
    {
        _indexRebuilder = indexRebuilder;
        _cacheService = cacheService;
        _options = options.Value;
        _examineManager = examineManager;
        _logger = logger;
        _umbracoContextFactory = umbracoContextFactory;
    }

    protected override async void Migrate()
    {

        if (!_options.Enabled)
        {
            _logger.LogDebug("FullTextSearch disabled - nothing gets reindexed");
            return;
        }

        if (!_examineManager.TryGetIndex(Constants.UmbracoIndexes.ExternalIndexName, out IIndex index))
        {
            _logger.LogDebug("FullTextSearch couldn't get ExternalIndex - nothing gets reindexed");
            return;
        }

        using (var cref = _umbracoContextFactory.EnsureUmbracoContext())
        {
            var nodes = cref.UmbracoContext.Content.GetAtRoot().ToList();

            foreach (var node in nodes)
            {
                _logger.LogDebug("Rendering and caching {nodeId}, {nodeName}", node.Id, node.Name);
                await _cacheService.AddTreeToCache(node);
            }

            _logger.LogDebug("Rebuilding index");

            index.CreateIndex();
            _indexRebuilder.RebuildIndex(Constants.UmbracoIndexes.ExternalIndexName);
        }
    }
}
