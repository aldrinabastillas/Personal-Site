using System.Collections.Generic;
using WebAppPortfolio.Models;

namespace WebAppPortfolio.Interfaces
{
    interface ISongList
    {
        /// <summary>
        /// Gets a list of Billboard Hot 100 songs from a given year
        /// </summary>
        /// <param name="year"></param>
        /// <returns>List of songs and artist ordered by chart position</returns>
        List<Billboard100Songs> GetYearList(int year);

        /// <summary>
        /// Gets a list of Billboard Hot 100 songs for all years
        /// </summary>
        /// <param name="year"></param>
        /// <returns>For each year, a list of songs and artist ordered by chart position</returns>
        Dictionary<int, List<Billboard100Songs>> GetAllYearLists();

    }
}
