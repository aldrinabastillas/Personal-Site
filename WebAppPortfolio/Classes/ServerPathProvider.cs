using System.Web;
using WebAppPortfolio.Interfaces;

namespace WebAppPortfolio.Classes
{
    public class ServerPathProvider : IPathProvider
    {
        public string MapPath(string path)
        {
            return HttpContext.Current.Server.MapPath(path);
        }
    }
}