using System.Web;
using System.Web.Optimization;

namespace WebAppPortfolio
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Vendor/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Vendor/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Vendor/modernizr-*",
                        "~/Vendor/startswith.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Vendor/bootstrap.js",
                      "~/Vendor/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Vendor/bootstrap.css",
                      "~/Content/Spotify.css",
                      "~/Content/_Visualizer.css"));

            bundles.Add(new ScriptBundle("~/bundles/SpotifyPage").Include(
                      "~/Vendor/angular.js",  //must come before Spotify.js
                      "~/Vendor/Semantic-UI/semantic.js",
                      "~/Scripts/Spotify/Spotify.js",
                      "~/Scripts/Spotify/tablesort.js"));

            BundleTable.EnableOptimizations = true; //minify. overrides compilation optimizations in web.config
        }
    }

}
