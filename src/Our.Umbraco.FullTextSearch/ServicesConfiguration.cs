using Microsoft.Extensions.DependencyInjection;
using Our.Umbraco.FullTextSearch.Helpers;
using Our.Umbraco.FullTextSearch.Interfaces;
using Our.Umbraco.FullTextSearch.Migrations;
using Our.Umbraco.FullTextSearch.NotificationHandlers;
using Our.Umbraco.FullTextSearch.Options;
using Our.Umbraco.FullTextSearch.Rendering;
using Our.Umbraco.FullTextSearch.Services;
using System;
using System.Net.Http;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Extensions;

namespace Our.Umbraco.FullTextSearch
{
    public static class ServicesConfiguration
    {
        public static IUmbracoBuilder AddFullTextSearch(this IUmbracoBuilder builder)
        {
            builder.Services.AddUnique<ICacheService, CacheService>();
            builder.Services.AddUnique<IHtmlService, HtmlService>();
            builder.Services.AddUnique<IStatusService, StatusService>();
            builder.Services.AddUnique<IPageRenderer, HttpPageRenderer>();
            builder.Services.AddScoped<ISearchService, SearchService>();
            builder.Services.AddScoped<FullTextSearchHelper>();

            builder.Services.AddHttpClient(FullTextSearchConstants.HttpClientFactoryNamedClientName)
            .ConfigurePrimaryHttpMessageHandler(x => new HttpClientHandler()
            {
                AllowAutoRedirect = false // Needed to now index 404, 301 pages etc.
            });


            builder
                .AddNotificationHandler<UmbracoApplicationStartingNotification, ExecuteMigrations>()
                .AddNotificationAsyncHandler<ContentCacheRefresherNotification, UpdateCacheOnPublish>()
                .AddNotificationHandler<ServerVariablesParsingNotification, AddFullTextSearchToServerVariables>()
                .AddNotificationHandler<UmbracoApplicationStartingNotification, AddFullTextItemsToIndex>();

            return builder;
        }

        public static IUmbracoBuilder AddFullTextSearch(this UmbracoBuilder builder, Action<FullTextSearchOptions> options)
        {
            if (options is not null)
            {
                builder.Services.Configure<FullTextSearchOptions>(options);
            }

            builder.AddFullTextSearch();

            return builder;
        }
    }
}
