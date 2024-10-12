using Our.Umbraco.FullTextSearch.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Our.Umbraco.FullTextSearch.Interfaces;

public interface ICacheService
{
    Task AddToCache(int id);
    Task AddToCache(IPublishedContent publishedContent);
    Task AddTreeToCache(IPublishedContent rootNode);
    Task AddTreeToCache(int rootId);
    bool CacheTableExists();
    Task DeleteFromCache(int id);
    Task<List<CacheItem>> GetFromCache(int id);
}