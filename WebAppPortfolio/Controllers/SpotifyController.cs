using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Configuration;
using System.Web.Mvc;
using WebAppPortfolio.Models;
using WebAppPortfolio.Classes;

namespace WebAppPortfolio.Controllers
{
    public class SpotifyController : HomeController
    {
        #region Properties
        private static EventLogger logger { get; set; }
        #endregion

        #region Public Actions
        /// <summary>
        /// Returns the app's main page in Views/Spotify/Index.cshtml
        /// Caches the Spotify Web API access token and the page for 5 minutes
        /// </summary>
        /// <returns></returns>
        [OutputCache(Duration = 300)]
        public override ActionResult Index()
        {
            string accessToken = SpotifyAPIs.GetSpotifyAccessToken();
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                HttpRuntime.Cache["AccessToken"] = accessToken;
            }

            //logger = new EventLogger();

            return View("Index", "_Layout");
        }

        /// <summary>
        /// Given a trackID, gets its audio features then passes that
        /// to the Machine Learning Web Service
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<JsonResult> GetPrediction(string id)
        {
            Task<JObject> audioFeatures = SpotifyAPIs.GetSpotifyAudioFeaturesAsync(id);
            Task<int> trackYear = SpotifyAPIs.GetTrackReleaseYear(id);
            await Task.WhenAll(audioFeatures, trackYear);

            string response;
            if(audioFeatures.Result != null && trackYear.Result != 0)
            {
                var scoreRequest = PredictionRequest.CreateRequest(audioFeatures.Result, trackYear.Result);
                response = await CallMlService(scoreRequest);
            }
            else
            {
                response = "Try again: Spotify Web APIs returned an error";
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets a list of Billboard Hot 100 songs from a given year
        /// from the SQL DB using Entity
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public JsonResult GetYearList(int year)
        {
            var list = new List<Billboard100Songs>();
            var connectionString = WebConfigurationManager.ConnectionStrings;
            using(var db = new Billboard100SongsAzure())
            {
                try
                {
                    list = (from songs in db.Billboard100
                            where songs.Year == year
                            select songs).OrderBy(s => s.Position).ToList();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.InnerException);
                }

            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Private Machine Learning Methods
        /// <summary>
        /// Calls the machine learning web service
        /// See https://docs.microsoft.com/en-us/azure/machine-learning/machine-learning-consume-web-services#request-response-service-rrs
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static async Task<string> CallMlService(PredictionRequest request)
        {
            string result = string.Empty;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(WebConfigurationManager.AppSettings["F1ModelUri"] + "&format=swagger");
                string apiKey = WebConfigurationManager.AppSettings["F1ModelApiKey"];
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                HttpResponseMessage response = await client.PostAsJsonAsync("", request).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    //LogWarning("Failed with status code: " + response.StatusCode);
                    result = "Try again: Microsoft Azure Machine Learning web service returned an error.";
                }
            }
            return result;
        }
        #endregion

        #region Error Handlers
        //protected override void OnException(ExceptionContext exceptionContext)
        //{
        //    base.OnException(exceptionContext); //call handler in HomeController
        //}

        //private static void LogWarning(string message)
        //{
        //    if (logger == null)
        //    {
        //        logger = new EventLogger();
        //    }

        //    logger.LogWarning(message);
        //}

        #endregion

    }
}
