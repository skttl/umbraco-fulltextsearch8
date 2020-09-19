using System.Collections.Generic;
using Our.Umbraco.FullTextSearch.Models;

namespace Our.Umbraco.FullTextSearch.Interfaces
{
    public interface ICacheService
    {
        void AddToCache(int id);
        void DeleteFromCache(int id);
        List<CacheItem> GetFromCache(int id);
    }
}