using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;
using WebAppPortfolio.Classes;
using WebAppPortfolio.Models;

namespace WebAppPortfolio.Controllers
{
    public class SpotifyController : HomeController
    {
        #region Properties
        private static EventLogger Logger { get; set; }
        #endregion

        #region Public Actions
        /// <summary>
        /// Returns the app's main page in Views/Spotify/Index.cshtml
        /// Caches the Spotify Web API access token and the page for 5 minutes
        /// </summary>
        /// <returns></returns>
        #if (!DEBUG)
                [OutputCache(Duration = 300)]
        #endif
        public override ViewResult Index()
        {
            SpotifyAPIs.GetSpotifyAccessToken(); //loads access token into runtime cache

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
            if (audioFeatures.Result != null && trackYear.Result != 0)
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
            //var redisLookup = new RedisSongList(new AzureSQLSongList(), new AzureRedisCache());
            var redisLookup = new RedisSongList(new AwsSQLSongList(), new AwsRedisCache());
            var list = redisLookup.GetYearList(year);

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Asynchronously pre-load all year lists into the Redis cache 
        /// Gets called upon page load in Spotify.js
        /// </summary>
        /// <returns></returns>
        public EmptyResult PreloadLists()
        {
            Task<Dictionary<int, List<Billboard100Songs>>> t = Task.Run(() =>
            {
                var redisLookup = new RedisSongList(new AwsSQLSongList(), new AwsRedisCache());
                return redisLookup.GetAllYearLists();
            });

            return null;
        }
        #endregion

        #region Non-Public Methods
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
                    Trace.TraceWarning("Failed with status code: " + response.StatusCode);
                    result = "Try again: Microsoft Azure Machine Learning web service returned an error.";
                }
            }
            return result;
        }

        /// <summary>
        /// Testing method that reads directly from the DB in Azure without caching
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public JsonResult GetYearListFromAzure(int year)
        {
            return GetYearListFromDb(new AzureSQLSongList(), year);
        }

        /// <summary>
        /// Testing method that reads directly from the DB in AWS without caching
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public JsonResult GetYearListFromAws(int year)
        {
            return GetYearListFromDb(new AwsSQLSongList(), year);
        }

        /// <summary>
        /// Testing method that reads directly from the specified DB without caching
        /// </summary>
        /// <param name="db"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        private JsonResult GetYearListFromDb(DBSongList db, int year)
        {
            var list = db.GetYearList(year);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Error Handlers
        protected override void OnException(ExceptionContext exceptionContext)
        {
            base.OnException(exceptionContext); //call base handler in HomeController
        }
        #endregion

    }
}
