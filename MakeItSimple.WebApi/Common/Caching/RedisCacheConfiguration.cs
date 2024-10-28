using StackExchange.Redis;

namespace MakeItSimple.WebApi.Common.Caching
{
    public static  class RedisCacheConfiguration
    {
        public static void AddRedisCache(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(connectionString));
        }

    }
}
