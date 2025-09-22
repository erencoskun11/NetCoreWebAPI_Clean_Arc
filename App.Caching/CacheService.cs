using App.Application.Contracts.Caching;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Caching
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;

        public CacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public Task AddAsync<T>(string cacheKey, T value, TimeSpan exprTimeSpan)
        {
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = exprTimeSpan
            };
            _memoryCache.Set(cacheKey, value, cacheOptions);
            return Task.CompletedTask;
        }

        public Task<T> GetAsync<T>(string cacheKey)
        {
            if (_memoryCache.TryGetValue(cacheKey, out T cacheItem)) return Task.FromResult(cacheItem);
            return Task.FromResult(default(T));
        }

        public Task RemoveAsync(string cacheKey)
        {
            _memoryCache.Remove(cacheKey);
            return Task.CompletedTask;
        }
    }
}
