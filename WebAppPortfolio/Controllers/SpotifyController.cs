using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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
            
            var scoreRequest = PredictionRequest.CreateRequest(audioFeatures.Result, trackYear.Result);

            string response = await CallMlService(scoreRequest);

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        private static async Task<string> CallMlService(PredictionRequest request)
        {
            string result = string.Empty;
            using (var client = new HttpClient())
            {
                string apiKey = WebConfigurationManager.AppSettings["AzureMlApiKey"];
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                client.BaseAddress = new Uri("https://ussouthcentral.services.azureml.net/workspaces/90569c39ed404120800dca5909257988/services/96fe7ad3758848b8a322f7be55dd6c9c/execute?api-version=2.0&format=swagger");

                HttpResponseMessage response = await client.PostAsJsonAsync("", request).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    //Console.WriteLine("Failed with status code: {0}", response.StatusCode);
                    result = "Error in calling prediction service, try again.";
                }
            }
            return result;
        }

        #region Private Spotify Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="song"></param>
        /// <param name="artist"></param>
        /// <returns></returns>
        private static async Task<string> SearchTrackIdAsync(string song, string artist)
        {
            string query = "?q=track:" + ParseSearchString(song) +
                           "%20artist:" + ParseSearchString(artist) +
                           "&type=track";
            Uri uri = new Uri("https://api.spotify.com/v1/search" + query);

            string trackId = string.Empty;
            //JObject response = await SendAsyncSpotifyQuery(uri);

            //JToken results = response.SelectToken("$.tracks.total");
            //if (results.ToString() != null && Convert.ToInt32(results.ToString()) > 0)
            //{
            //    JToken id = response.SelectToken("$.tracks.items[0].id");
            //    trackId = id.ToString();
            //}

            return trackId;
        }

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
                    Console.WriteLine(ex.Message);
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
                    throw new Exception(ex.Message + " in GetAccessToken()");
                }
                return accessToken;
            }
        }
        #endregion

        #region Misc. Private Helper Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="raw"></param>
        /// <returns></returns>
        private static string ParseSearchString(string raw)
        {
            raw.Replace('#', ' ');
            if (raw.Contains(" feat."))
            {
                string[] stringSeperator = new string[] { "feat." };
                string[] temp = raw.Split(stringSeperator, StringSplitOptions.None);
                return temp[0];
            }
            return raw;
        }
        #endregion

    }
}
