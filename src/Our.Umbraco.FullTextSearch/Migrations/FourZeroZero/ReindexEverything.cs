using Examine;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Our.Umbraco.FullTextSearch.Interfaces;
using Our.Umbraco.FullTextSearch.Options;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Services.Navigation;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Examine;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Extensions;

namespace Our.Umbraco.FullTextSearch.Migrations.FourZeroZero;

public class ReindexEverything : AsyncMigrationBase
{
    private readonly IDocumentNavigationQueryService _documentNavigationQueryService;
    private readonly IUmbracoContextFactory _umbracoContextFactory;
    private readonly IIndexRebuilder _indexRebuilder;
    private readonly ICacheService _cacheService;
    private readonly FullTextSearchOptions _options;
    private readonly IExamineManager _examineManager;
    private readonly ILogger<ReindexEverything> _logger;

    public ReindexEverything(IMigrationContext context, IIndexRebuilder indexRebuilder, ICacheService cacheService, IOptions<FullTextSearchOptions> options, IExamineManager examineManager, ILogger<ReindexEverything> logger, IUmbracoContextFactory umbracoContextFactory, IDocumentNavigationQueryService documentNavigationQueryService) : base(context)
    {
        _indexRebuilder = indexRebuilder;
        _cacheService = cacheService;
        _options = options.Value;
        _examineManager = examineManager;
        _logger = logger;
        _umbracoContextFactory = umbracoContextFactory;
        _documentNavigationQueryService = documentNavigationQueryService;
    }

    protected override async Task MigrateAsync()
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

        using var cref = _umbracoContextFactory.EnsureUmbracoContext();
        if (_documentNavigationQueryService.TryGetRootKeys( out var rootKeys))
        {
            var nodes = (await Task.WhenAll(rootKeys.Select(x => cref.UmbracoContext.Content.GetByIdAsync(x)))).WhereNotNull().ToList();
            foreach (var node in nodes)
            {
                _logger.LogDebug("Rendering and caching {nodeId}, {nodeName}", node.Id, node.Name);
                await _cacheService.AddTreeToCache(node);
            }

            _logger.LogDebug("Rebuilding index");

            index.CreateIndex();
            await _indexRebuilder.RebuildIndexAsync(Constants.UmbracoIndexes.ExternalIndexName);
        }
    }
}
