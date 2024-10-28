using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace MakeItSimple.WebApi.Common.Caching
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;

        public CacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task SetCacheAsync(string key, object value, TimeSpan expiration)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(value);
            await _distributedCache.SetStringAsync(key, json, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            });
        }

        public async Task<object> GetCacheAsync(string key)
        {
            var json = await _distributedCache.GetStringAsync(key);
            return json == null ? null : System.Text.Json.JsonSerializer.Deserialize<object>(json);
        }

        public async Task RemoveCacheAsync(string key)
        {
            await _distributedCache.RemoveAsync(key);
        }
    }
}
