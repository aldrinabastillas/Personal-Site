using StackExchange.Redis;
using System;
using System.Configuration;

namespace WebAppPortfolio.Classes
{
    public abstract class RedisCache
    {
        private static Lazy<ConnectionMultiplexer> _lazyConnection;

        public RedisCache(string connectionKey)
        {
            string cacheConnection = ConfigurationManager.AppSettings[connectionKey].ToString();
            _lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                return ConnectionMultiplexer.Connect(cacheConnection);
            });
        }

        public virtual ConnectionMultiplexer Connection
        {
            get
            {
                return _lazyConnection.Value;
            }
        }

        public virtual IDatabase Cache
        {
            get
            {
                return Connection.GetDatabase();
            }
        }

        public virtual IServer Server
        {
            get
            {
                var servers = Connection.GetEndPoints();
                return Connection.GetServer(servers[0]); //currently only 1 Redis server end point configured, could change with clustering
            }
        }
    }
}