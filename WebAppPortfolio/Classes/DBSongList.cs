using System.Collections.Generic;
using System.Configuration;
using System.Web.Configuration;
using WebAppPortfolio.Interfaces;
using WebAppPortfolio.Models;

namespace WebAppPortfolio.Classes
{
    /// <summary>
    /// An abstract interface to load the song lists from a database
    /// </summary>
    public abstract class DBSongList : ISongList
    {
        #region Connection
        protected virtual ConnectionStringSettingsCollection connection
        {
            get
            {
                return WebConfigurationManager.ConnectionStrings;
            }
        }
        #endregion

        #region Interface Methods
        public abstract Dictionary<int, List<Billboard100Songs>> GetAllYearLists();

        public abstract List<Billboard100Songs> GetYearList(int year); 
        #endregion
    }
}