using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using WebAppPortfolio.Interfaces;
using WebAppPortfolio.Models;

namespace WebAppPortfolio.Classes
{
    public class RedisSongList : ISongList
    {
        #region Redis Cache Connections
        private static IDatabase Cache { get; set; }
        private static IServer Server { get; set; }
        public static DBSongList DbLookup { get; set; }
        #endregion

        public RedisSongList(DBSongList db, RedisCache cache)
        {
            Cache = cache.Cache;
            Server = cache.Server;
            DbLookup = db;
        }

        #region Interface Methods
        /// <summary>
        /// Gets a list of Billboard Hot 100 songs from a given year
        /// from the Redis Cache. On cache misses, will get from SQL and store in cache.
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public List<Billboard100Songs> GetYearList(int year)
        {
            string cacheKey = "yearList:" + year;
            string serializedList = Cache.StringGet(cacheKey);
            var list = new List<Billboard100Songs>();
            
            if(!string.IsNullOrEmpty(serializedList)){
                //in cache, deserialize
                list = JsonConvert.DeserializeObject<List<Billboard100Songs>>(serializedList);
            }
            else
            {
                //not in cache, get from DB and store in cache
                list = DbLookup.GetYearList(year);

                Cache.StringSet(cacheKey, JsonConvert.SerializeObject(list));
            }

            return list;
        }

        /// <summary>
        /// Gets a list of Billboard Hot 100 songs for all years
        /// from the Redis Cache. On cache misses, will get from SQL and store in cache.
        /// </summary>
        /// <param name="year"></param>
        /// <returns>Dictionary where key = year, value = list of songs and artist ordered by chart position</returns>
        public Dictionary<int, List<Billboard100Songs>> GetAllYearLists()
        {
            var lists = new Dictionary<int, List<Billboard100Songs>>();

            //load all lists from cache
            foreach (var key in Server.Keys(pattern: "*yearList:*"))
            {
                string year = key.ToString().Replace("yearList:", "");
                string serializedList = Cache.StringGet(year);
                var list = new List<Billboard100Songs>();
                if (!string.IsNullOrEmpty(serializedList))
                {
                    //in cache, deserialize and add to dictionary
                    list = JsonConvert.DeserializeObject<List<Billboard100Songs>>(serializedList);
                    lists.Add(Convert.ToInt32(year), list);
                }
            }

            //if all lists aren't loaded, get all from DB and cache
            if (lists.Keys.Count < 70) //TODO: Don't hard code the count of lists
            {
                lists = DbLookup.GetAllYearLists();

                foreach (var year in lists.Keys)
                {
                    Cache.StringSet("yearList:" + year, JsonConvert.SerializeObject(lists[year]));
                }
            }

            return lists;
        }
        #endregion
    }
}