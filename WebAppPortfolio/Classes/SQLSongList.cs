using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Configuration;
using WebAppPortfolio.Interfaces;
using WebAppPortfolio.Models;

namespace WebAppPortfolio.Classes
{
    public class SQLSongList : ISongList
    {
        #region Connection
        private ConnectionStringSettingsCollection connection = WebConfigurationManager.ConnectionStrings;
        #endregion

        #region Interface Methods
        /// <summary>
        /// Gets a list of Billboard Hot 100 songs from a given year
        /// from the SQL DB using Entity
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public List<Billboard100Songs> GetYearList(int year)
        {
            var list = new List<Billboard100Songs>();
            using (var db = new Billboard100SongsAzure())
            {
                try
                {
                    list = (from songs in db.Billboard100
                            where songs.Year == year
                            select songs).OrderBy(s => s.Position).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.InnerException);
                }

            }
            return list;
        }

        /// <summary>
        /// Gets a list of Billboard Hot 100 songs for all years
        /// from the SQL DB using Entity
        /// </summary>
        /// <param name="year"></param>
        /// <returns>Dictionary where key = year, value = list of songs and artist ordered by chart position</returns>
        public Dictionary<int, List<Billboard100Songs>> GetAllYearLists()
        {
            var lists = new Dictionary<int, List<Billboard100Songs>>();
            using (var db = new Billboard100SongsAzure())
            {
                try
                {
                    var result = (from songs in db.Billboard100
                                  group songs by songs.Year into yearList
                                  orderby yearList.Key
                                  select yearList).ToList();

                    foreach (var group in result)
                    {
                        lists.Add(group.Key, group.OrderBy(g => g.Position).ToList());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.InnerException);
                }
            }
            return lists;
        }
        #endregion
    }
}