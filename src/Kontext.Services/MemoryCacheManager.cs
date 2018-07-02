using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;

namespace Kontext.Services
{
    /// <summary>
    /// Caching manager
    /// </summary>
    public class MemoryCacheManager : ICacheManager
    {
        private readonly IMemoryCache memoryCache;
        private List<CachedEntry> cachedItems;

        public MemoryCacheManager(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
            cachedItems = new List<CachedEntry>();
        }

        public IEnumerable<CachedEntry> GetAllCachedEntries()
        {
            return cachedItems;
        }

        public void Remove(object key)
        {
            memoryCache.Remove(key);
            RemoveIfExists(key);
        }

        private void RemoveIfExists(object key)
        {
            var entry = cachedItems.Find(e => e.CacheKey.CompareTo(key.ToString()) == 0);
            if (cachedItems.Exists(e => e.CacheKey.CompareTo(key.ToString()) == 0))
                cachedItems.Remove(entry);
        }

        public TItem SetAbsoluteExpiration<TItem>(object key, TItem value, TimeSpan absoluteExpiration)
        {
            // Set cache options.
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                // Keep in cache for this time, reset time if accessed.
                .SetAbsoluteExpiration(absoluteExpiration);

            RemoveIfExists(key);
            cachedItems.Add(new CachedEntry(key.ToString(), DateTime.Now));

            return memoryCache.Set(key, value, cacheEntryOptions);
        }

        public TItem SetSlidingExpiration<TItem>(object key, TItem value, TimeSpan relativeExpiration)
        {
            // Set cache options.
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                // Keep in cache for this time, reset time if accessed.
                .SetSlidingExpiration(relativeExpiration);

            RemoveIfExists(key);
            cachedItems.Add(new CachedEntry(key.ToString(), DateTime.Now));

            return memoryCache.Set(key, value, cacheEntryOptions);
        }

        public bool TryGetValue<TItem>(object key, out TItem value)
        {
            return memoryCache.TryGetValue(key, out value);
        }
    }
}
