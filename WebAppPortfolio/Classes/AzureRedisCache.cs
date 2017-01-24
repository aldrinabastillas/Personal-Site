using StackExchange.Redis;

namespace WebAppPortfolio.Classes
{
    public class AzureRedisCache : RedisCache
    {
        public AzureRedisCache() : base("AzureRedisCacheConnection")
        {
        }
        
        public override IServer Server
        {
            get
            {
                var servers = Connection.GetEndPoints(); 
                return Connection.GetServer(servers[0]); //currently only 1 Redis server end point configured
            }
        }

    }
}