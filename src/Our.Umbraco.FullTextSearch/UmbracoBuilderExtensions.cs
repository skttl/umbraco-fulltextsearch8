using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace Our.Umbraco.FullTextSearch;

internal static class UmbracoBuilderExtensions
{
    /// <summary>
    /// Registers a notification handler that will be executed before another handler that was already registered.
    /// This method must be called after the other handler has already been registered.
    /// </summary>
    /// <typeparam name="TNotification">The type of notification.</typeparam>
    /// <typeparam name="TBeforeNotificationHandler">The other type of the notification handler that must run after the newly added</typeparam>
    /// <typeparam name="TNotificationHandler">The type of notification handler.</typeparam>
    /// <returns>The <see cref="IUmbracoBuilder"/>.</returns>
    public static IUmbracoBuilder AddNotificationHandlerBefore<TNotification, TBeforeNotificationHandler, TNotificationHandler>(this IUmbracoBuilder builder)
        where TNotificationHandler : INotificationHandler<TNotification>
        where TNotification : INotification
        where TBeforeNotificationHandler : INotificationHandler<TNotification>
    {

        var descriptor = new UniqueServiceDescriptor(typeof(INotificationHandler<TNotification>), typeof(TBeforeNotificationHandler), ServiceLifetime.Transient);

        bool shouldReInsertBeforeHandler = false;

        if (builder.Services.Contains(descriptor))
        {
            builder.Services.Remove(descriptor);
            shouldReInsertBeforeHandler = true;
        }

        builder.AddNotificationHandler<TNotification, TNotificationHandler>();

        // Make sure that TBeforeNotificationHandler is inserted after TNotificationHandler
        // so that it runs after TNotificationHandler
        if (shouldReInsertBeforeHandler)
        {
            builder.Services.Add(descriptor);
        }

        return builder;
    }
}