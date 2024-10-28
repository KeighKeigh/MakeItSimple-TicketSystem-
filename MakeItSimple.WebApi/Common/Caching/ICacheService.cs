namespace MakeItSimple.WebApi.Common.Caching
{
    public interface ICacheService
    {
        Task SetAsync(string key, object value, TimeSpan? expiration = null);
        Task<object> GetAsync(string key);
        Task RemoveAsync(string key);

        Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null);

    }
}
