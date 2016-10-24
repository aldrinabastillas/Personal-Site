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

namespace WebAppPortfolio.Controllers
{
    public class SpotifyController : Controller
    {
        #region Public Actions
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        public ActionResult Index()
        {
            string accessToken = GetSpotifyAccessToken();
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                HttpRuntime.Cache["AccessToken"] = accessToken;
            }

            return View("Index", "_Layout");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<JsonResult> GetPrediction(string id)
        {
            Task<JObject> audioFeatures = GetSpotifyAudioFeaturesAsync(id);
            Task<int> trackYear = GetTrackReleaseYear(id);
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
        /// 
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public JsonResult GetYearList(int year)
        {
            var list = new List<Billboard100Song>();
            using (var db = new Billboard100Entities())
            {
                try
                {
                    list = (from songs in db.Billboard100Song
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
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static async Task<string> CallMlService(PredictionRequest request)
        {
            string result = string.Empty;
            using (var client = new HttpClient())
            {
                //string apiKey = WebConfigurationManager.AppSettings["AucModelApiKey"];
                //client.BaseAddress = new Uri(WebConfigurationManager.AppSettings["AucModelUri"] + "&format=swagger");

                string apiKey = WebConfigurationManager.AppSettings["F1ModelApiKey"];
                client.BaseAddress = new Uri(WebConfigurationManager.AppSettings["F1ModelUri"] + "&format=swagger");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                HttpResponseMessage response = await client.PostAsJsonAsync("", request).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    Console.WriteLine("Failed with status code: {0}", response.StatusCode);
                    result = "Try again: Microsoft Azure Machine Learning web service returned an error.";
                }
            }
            return result;
        }
        #endregion

        #region Private Spotify Methods
        /// <summary>
        /// See https://developer.spotify.com/web-api/get-audio-features/
        /// Calls https://api.spotify.com/v1/audio-features/{id}
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private static async Task<JObject> GetSpotifyAudioFeaturesAsync(string id)
        {
            var uri = new Uri("https://api.spotify.com/v1/audio-features/" + id);
            return await SendAsyncSpotifyQuery(uri); ;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trackId"></param>
        /// <returns></returns>
        private static async Task<int> GetTrackReleaseYear(string trackId)
        {
            string albumId = await GetAlbumIdAsync(trackId);
            return await GetAlbumReleaseYear(albumId);
        }

        /// <summary>
        /// See https://developer.spotify.com/web-api/get-track/
        /// Calls https://api.spotify.com/v1/tracks/{id}
        /// </summary>
        /// <param name="trackId"></param>
        /// <returns></returns>
        private static async Task<string> GetAlbumIdAsync(string trackId)
        {
            Uri uri = new Uri("https://api.spotify.com/v1/tracks/" + trackId);
            JObject response = await SendAsyncSpotifyQuery(uri);

            string albumId = string.Empty;
            if (response.HasValues)
            {
                albumId = response.SelectToken("$.album.id").ToString();
            }

            return albumId;
        }

        /// <summary>
        /// See https://developer.spotify.com/web-api/get-album/
        /// Calls 	https://api.spotify.com/v1/albums/{id}
        /// </summary>
        /// <param name="albumId"></param>
        /// <returns></returns>
        private static async Task<int> GetAlbumReleaseYear(string albumId)
        {
            Uri uri = new Uri("https://api.spotify.com/v1/albums/" + albumId);
            JObject response = await SendAsyncSpotifyQuery(uri);

            int date = 0;
            if (response.HasValues)
            {
                string[] temp = response.SelectToken("$.release_date").ToString().Split('-');
                date = Convert.ToInt32(temp[0]);
            }

            return date;
        }

        /// <summary>
        /// For error object model, see https://developer.spotify.com/web-api/user-guide/#error-details
        /// For info about request limits, see https://developer.spotify.com/web-api/user-guide/#rate-limiting
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        private static async Task<JObject> SendAsyncSpotifyQuery(Uri uri)
        {
            var obj = new JObject();
            using (var client = new WebClient())
            {
                try
                {
                    string token = GetSpotifyAccessToken();
                    client.Headers.Add("Authorization", "Bearer " + token);
                    string response = await client.DownloadStringTaskAsync(uri);
                    obj = JsonConvert.DeserializeObject<JObject>(response);
                }
                catch (WebException ex)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)ex.Response;
                    if (httpResponse.StatusDescription.Contains("Too Many Requests"))
                    {
                        int seconds = Convert.ToInt32(ex.Response.Headers["Retry-After"]);
                        seconds = (seconds < 5) ? 5 : seconds; //if seconds < 5, default to 5

                        Console.WriteLine("Will retry after {0} second(s)", seconds);
                        Thread.Sleep(seconds * 1000);
                        Console.WriteLine("Retrying");
                        return await SendAsyncSpotifyQuery(uri);
                    }
                    else
                    {
                        Trace.WriteLine(ex); //write to errors.xml file specified in Web.config
                    }
                }
            }
            return obj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static string GetSpotifyAccessToken()
        {
            string accessToken = HttpRuntime.Cache["AccessToken"] as string;
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                return accessToken;
            }

            Uri uri = new Uri("https://accounts.spotify.com/api/token");
            string privateKey = WebConfigurationManager.AppSettings["SpotifyPrivateAppKey"];
            string clientID = WebConfigurationManager.AppSettings["SpotifyClientID"];

            using (var client = new WebClient())
            {
                string authValue = clientID + ":" + privateKey;
                byte[] bytes = Encoding.UTF8.GetBytes(authValue);
                authValue = Convert.ToBase64String(bytes);

                client.Headers.Add("Authorization", "Basic " + authValue);

                NameValueCollection data = new NameValueCollection
                {
                    { "grant_type", "client_credentials" }
                };

                byte[] response;
                try
                {
                    response = client.UploadValues(uri, "POST", data);
                    JObject obj = JsonConvert.DeserializeObject<JObject>(Encoding.Default.GetString(response));
                    if (obj.HasValues)
                    {
                        JToken jToken = obj.SelectToken("$.access_token");
                        accessToken = jToken.ToString();
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex); //write to errors.xml file specified in Web.config
                }
                return accessToken;
            }
        }
        #endregion

    }
}
