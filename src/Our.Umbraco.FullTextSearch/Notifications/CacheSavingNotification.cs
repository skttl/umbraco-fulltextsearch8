using Our.Umbraco.FullTextSearch.Services.Models;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace Our.Umbraco.FullTextSearch.Notifications;

public class CacheSavingNotification : SavingNotification<CacheItem>
{
    public CacheSavingNotification(CacheItem target, EventMessages messages) : base(target, messages)
    {
    }
}
