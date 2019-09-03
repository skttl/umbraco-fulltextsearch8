using System.Collections.Generic;
using Our.Umbraco.FullTextSearch.Models;

namespace Our.Umbraco.FullTextSearch.Interfaces
{
    public interface ICacheService
    {
        void AddCacheTask(int id);
        void AddToCache(int id);
        void DeleteCacheTask(int taskId);
        void DeleteFromCache(int id);
        List<CacheTask> GetCacheTasks();
        List<CacheTask> GetAllCacheTasks();
        List<CacheItem> GetFromCache(int id);
        void SetTaskAsStarted(CacheTask task);
    }
}