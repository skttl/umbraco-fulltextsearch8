using Examine;
using Our.Umbraco.FullTextSearch.Interfaces;
using Our.Umbraco.FullTextSearch.Services;
using System;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Logging;
using Umbraco.Core.Services;
using Umbraco.Examine;
using Umbraco.Web.Scheduling;

namespace Our.Umbraco.FullTextSearch.Components
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class PerformCacheTasksComposer : ComponentComposer<PerformCacheTasksComponent>
    {
    }

    public class PerformCacheTasksComponent : IComponent
    {
        private readonly ICacheService _cacheService;
        private readonly FullTextSearchConfig _fullTextConfig;
        private readonly IPublishedContentValueSetBuilder _valueSetBuilder;
        private readonly IExamineManager _examineManager;
        private readonly IProfilingLogger _profilingLogger;
        private readonly ILogger _logger;
        private readonly IContentService _contentService;
        private readonly BackgroundTaskRunner<IBackgroundTask> _performCacheTasksRunner;

        public PerformCacheTasksComponent(
            IProfilingLogger profilingLogger,
            ILogger logger,
            IContentService contentService,
            ICacheService cacheService,
            IExamineManager examineManager,
            IPublishedContentValueSetBuilder valueSetBuilder,
            FullTextSearchConfig fullTextConfig)
        {
            _profilingLogger = profilingLogger;
            _logger = logger;
            _contentService = contentService;
            _cacheService = cacheService;
            _examineManager = examineManager;
            _valueSetBuilder = valueSetBuilder;
            _fullTextConfig = fullTextConfig;
            _performCacheTasksRunner = new BackgroundTaskRunner<IBackgroundTask>("PerformCacheTasks", _logger);
        }
        public void Initialize()
        {

            var task = new PerformCacheTasks(_performCacheTasksRunner, 1000, 10000, _profilingLogger, _logger, _contentService, _cacheService, _examineManager, _valueSetBuilder, _fullTextConfig);

            _performCacheTasksRunner.TryAdd(task);
        }

        public void Terminate()
        {
        }
    }
    public class PerformCacheTasks : RecurringTaskBase
    {
        private readonly IProfilingLogger _profilingLogger;
        private readonly ICacheService _cacheService;
        private readonly FullTextSearchConfig _fullTextConfig;
        private readonly IPublishedContentValueSetBuilder _valueSetBuilder;
        private readonly IExamineManager _examineManager;
        private readonly ILogger _logger;
        private readonly IContentService _contentService;

        public PerformCacheTasks(IBackgroundTaskRunner<RecurringTaskBase> runner,
            int delayMilliseconds,
            int periodMilliseconds,
            IProfilingLogger profilingLogger,
            ILogger logger,
            IContentService contentService,
            ICacheService cacheService,
            IExamineManager examineManager,
            IPublishedContentValueSetBuilder valueSetBuilder,
            FullTextSearchConfig fullTextConfig)
            : base(runner, delayMilliseconds, periodMilliseconds)
        {
            _valueSetBuilder = valueSetBuilder;
            _examineManager = examineManager;
            _cacheService = cacheService;
            _fullTextConfig = fullTextConfig;
            _contentService = contentService;
            _logger = logger;
            _profilingLogger = profilingLogger;
        }

        public override bool PerformRun()
        {
            if (!_fullTextConfig.Enabled) return false;
            if (!_examineManager.TryGetIndex("ExternalIndex", out IIndex index))
            {
                _logger.Error<PerformCacheTasks>(new InvalidOperationException("No index found by name ExternalIndex"));
                return false;
            }

            try
            {
                using (_profilingLogger.DebugDuration<PerformCacheTasks>("PerformCacheTasks", "PerformCacheTasks done"))
                {
                    var tasks = _cacheService.GetCacheTasks();

                    foreach (var task in tasks)
                    {
                        _cacheService.SetTaskAsStarted(task);
                    }

                    foreach (var task in tasks)
                    {
                        var content = _contentService.GetById(task.NodeId);
                        if (content != null)
                        {
                            _cacheService.AddToCache(task.NodeId);
                            index.IndexItems(_valueSetBuilder.GetValueSets(content));
                        }
                        else
                        {
                            _cacheService.DeleteFromCache(task.NodeId);
                        }
                        _cacheService.DeleteCacheTask(task.Id);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error<PerformCacheTasks>(e);
            }

            return true;
        }

        public override bool IsAsync => false;
    }
}
