using StackExchange.Redis;

namespace WebAppPortfolio.Classes
{
    public class AwsRedisCache : RedisCache
    {
        public AwsRedisCache() : base("AwsRedisCacheConnection")
        {
        }

        public override IServer Server
        {
            get
            {
                var servers = Connection.GetEndPoints();
                return Connection.GetServer(servers[0]); //currently only 1 Redis server end point configured, could change with clustering
            }
        }
    }
}