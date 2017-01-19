using StackExchange.Redis;
using System;
using System.Configuration;

namespace WebAppPortfolio.Classes
{
    public class RedisCache
    {
        /// <summary>
        /// Lazy initializer for a connection to the Redis cache
        /// </summary>
        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            string cacheConnection = ConfigurationManager.AppSettings["RedisCacheConnection"].ToString();
            return ConnectionMultiplexer.Connect(cacheConnection);
        });

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }

        public static IDatabase Cache
        {
            get
            {
                return Connection.GetDatabase();
            }
        }

        public static IServer Server
        {
            get
            {
                var servers = Connection.GetEndPoints(); 
                return Connection.GetServer(servers[0]); //currently only 1 Redis server end point configured
            }
        }
    }
}