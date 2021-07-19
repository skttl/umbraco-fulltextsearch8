using Our.Umbraco.FullTextSearch.Services.Models;
using System.Collections.Generic;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Our.Umbraco.FullTextSearch.Interfaces
{
    public interface ICacheService
    {
        void AddToCache(int id);
        void AddToCache(IPublishedContent publishedContent);
        void AddTreeToCache(IPublishedContent rootNode);
        void AddTreeToCache(int rootId);
        void DeleteFromCache(int id);
        List<CacheItem> GetFromCache(int id);
    }
}