namespace MakeItSimple.WebApi.Common.Caching
{
    public interface ICacheService
    {
        Task SetCacheAsync(string key, object value, TimeSpan expiration);
        Task<object> GetCacheAsync(string key);
        Task RemoveCacheAsync(string key);

    }
}
