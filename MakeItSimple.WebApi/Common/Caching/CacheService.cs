using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace MakeItSimple.WebApi.Common.Caching
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;

        public CacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public CacheService(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;
        }
        public async Task SetAsync(string key, object value, TimeSpan? expiration = null)
        {
            var db = _connectionMultiplexer.GetDatabase();
        }

        public async Task<object> GetAsync(string key)
        {
            var db = _connectionMultiplexer.GetDatabase();
            return null;
        }

        public async Task RemoveAsync(string key)
        {
            var db = _connectionMultiplexer.GetDatabase();
            await db.KeyDeleteAsync(key);
        }


        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
        {
            var db = _connectionMultiplexer.GetDatabase();

            var cachedValue = await db.StringGetAsync(key);
            if (cachedValue.HasValue)
            {
                return JsonConvert.DeserializeObject<T>(cachedValue);
            }

            var result = await factory();

            if (result != null)
            {
                await db.StringSetAsync(key, JsonConvert.SerializeObject(result), expiration);
            }

            return result;
        }
    }
}
