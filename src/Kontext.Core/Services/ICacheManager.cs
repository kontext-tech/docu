using System;
using System.Collections.Generic;

namespace Kontext.Services
{
    public struct CachedEntry
    {
        public CachedEntry(string cacheKey, DateTime addedDateTime) : this()
        {
            CacheKey = cacheKey ?? throw new ArgumentNullException(nameof(cacheKey));
            AddedDateTime = addedDateTime;
        }

        public string CacheKey { get; set; }
        public DateTime AddedDateTime { get; set; }
    }

    public interface ICacheManager
    {
        TItem SetAbsoluteExpiration<TItem>(object key, TItem value, TimeSpan absoluteExpiration);

        TItem SetSlidingExpiration<TItem>(object key, TItem value, TimeSpan relativeExpiration);

        bool TryGetValue<TItem>(object key, out TItem value);

        IEnumerable<CachedEntry> GetAllCachedEntries();

        void Remove(object key);
    }
}
