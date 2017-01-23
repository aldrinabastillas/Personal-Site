using System;
using System.Web.Mvc;
using System.Web.Routing;
using WebAppPortfolio.Classes;

namespace WebAppPortfolio
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "GetYearList",
                url: "Spotify/GetYearList/{year}",
                defaults: new {
                    controller = "Spotify",
                    action = "GetYearList",
                    year = DateTime.Now.Year - 2
                }
            );

            routes.MapRoute(
                name: "GetYearListFromSQL",
                url: "Spotify/GetYearListFromSQL/{year}",
                defaults: new
                {
                    controller = "Spotify",
                    action = "GetYearListFromSQL",
                    year = DateTime.Now.Year - 2
                }
            );

            routes.MapRoute(
                name: "Resume",
                url: "Home/Resume/{pathProvider}",
                defaults: new
                {
                    controller = "Home",
                    action = "Resume",
                    pathProvider = new ServerPathProvider()
                }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new {
                    controller = "Home",
                    action = "Index",
                    id = UrlParameter.Optional
                }
            );

            
        }
    }
}
