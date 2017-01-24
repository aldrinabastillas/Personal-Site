using StackExchange.Redis;

namespace WebAppPortfolio.Classes
{
    /// <summary>
    /// Implements the RedisCache abstract class
    /// </summary>
    public class AwsRedisCache : RedisCache
    {
        /// <summary>
        /// Calls the base constructor with the specified app settings key (see Web.config)
        /// </summary>
        public AwsRedisCache() : base("AwsRedisCacheConnection")
        {
        }

        /// <summary>
        /// May need to override if adding Redis nodes
        /// </summary>
        //public override IServer Server
        //{
            
        //}
    }
}