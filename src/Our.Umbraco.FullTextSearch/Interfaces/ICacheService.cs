using Our.Umbraco.FullTextSearch.Services.Models;
using System.Collections.Generic;

namespace Our.Umbraco.FullTextSearch.Interfaces
{
    public interface ICacheService
    {
        void AddToCache(int id);
        void DeleteFromCache(int id);
        List<CacheItem> GetFromCache(int id);
    }
}