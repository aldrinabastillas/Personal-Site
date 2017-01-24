using System;
using System.Collections.Generic;
using System.Linq;
using WebAppPortfolio.Models;

namespace WebAppPortfolio.Classes
{
    /// <summary>
    /// Implements the DBSongList abstract class, which implements the ISongList interface
    /// </summary>
    public class AwsSQLSongList : DBSongList
    {
        #region Interface Methods
        /// <summary>
        /// Gets a list of Billboard Hot 100 songs from a given year
        /// from the SQL DB in AWS using Entity
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public override List<Billboard100Songs> GetYearList(int year)
        {
            var list = new List<Billboard100Songs>();
            using (var db = new Billboard100SongsAWS())
            {
                try
                {
                    list = (from songs in db.Billboard100Songs
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
        /// from the SQL DB in AWS using Entity
        /// </summary>
        /// <param name="year"></param>
        /// <returns>Dictionary where key = year, value = list of songs and artist ordered by chart position</returns>
        public override Dictionary<int, List<Billboard100Songs>> GetAllYearLists()
        {
            var lists = new Dictionary<int, List<Billboard100Songs>>();
            using (var db = new Billboard100SongsAWS())
            {
                try
                {
                    var result = (from songs in db.Billboard100Songs
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