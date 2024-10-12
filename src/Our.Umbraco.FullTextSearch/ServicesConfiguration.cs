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
using Umbraco.Cms.Infrastructure.Search;
using Umbraco.Extensions;

namespace Our.Umbraco.FullTextSearch;

public static class ServicesConfiguration
{
    public static IUmbracoBuilder AddFullTextSearch(this IUmbracoBuilder builder, Action<FullTextSearchOptions> defaultOptions = default)
    {
        // load up the settings. 
        var options = builder.Services.AddOptions<FullTextSearchOptions>()
            .Bind(builder.Config.GetSection("Umbraco:FullTextSearch"));

        if (defaultOptions != default)
        {
            options.Configure(defaultOptions);
        }
        options.ValidateDataAnnotations();

        builder.Services.AddUnique<ICacheService, CacheService>();
        builder.Services.AddUnique<IHtmlService, HtmlService>();
        builder.Services.AddUnique<IPageRenderer, HttpPageRenderer>();
        builder.Services.AddUnique<IStatusService, StatusService>();
        builder.Services.AddScoped<ISearchService, SearchService>();
        builder.Services.AddScoped<FullTextSearchHelper>();

        builder.Services.AddHttpClient(FullTextSearchConstants.HttpClientFactoryNamedClientName)
        .ConfigurePrimaryHttpMessageHandler(x => new HttpClientHandler()
        {
            AllowAutoRedirect = false // Needed to know index 404, 301 pages etc.
        });


        builder
            .AddNotificationHandler<UmbracoApplicationStartingNotification, ExecuteMigrations>()
            .AddNotificationHandler<UmbracoApplicationStartingNotification, AddFullTextItemsToIndex>()
            .AddNotificationHandlerBefore<ContentCacheRefresherNotification, ContentIndexingNotificationHandler, UpdateCacheOnPublish>();

        return builder;
    }
}
