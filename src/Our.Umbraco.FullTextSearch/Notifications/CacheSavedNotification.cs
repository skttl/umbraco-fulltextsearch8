using Our.Umbraco.FullTextSearch.Services.Models;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace Our.Umbraco.FullTextSearch.Notifications;

public class CacheSavedNotification : SavedNotification<CacheItem>
{
    public CacheSavedNotification(CacheItem target, EventMessages messages) : base(target, messages)
    {
    }
}
