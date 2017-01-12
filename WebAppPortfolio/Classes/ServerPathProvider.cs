using System.Web;
using WebAppPortfolio.Interfaces;

namespace WebAppPortfolio.Classes
{
    public class ServerPathProvider : IPathProvider
    {
        /// <summary>
        /// Production implementation of IPathProvider.MapPath
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string MapPath(string path)
        {
            return HttpContext.Current.Server.MapPath(path);
        }
    }
}