using StackExchange.Redis;

namespace WebAppPortfolio.Classes
{
    public class AzureRedisCache : RedisCache
    {
        /// <summary>
        /// Implements the RedisCache abstract class
        /// </summary>
        public AzureRedisCache() : base("AzureRedisCacheConnection")
        {
        }
    }
}