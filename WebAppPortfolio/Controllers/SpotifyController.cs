using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
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

        #region Spotify Helper Functions
        /// <summary>
        /// 
        /// </summary>
        /// <param name="song"></param>
        /// <param name="artist"></param>
        /// <returns></returns>
        private async Task<string> SearchTrackIdAsync(string song, string artist)
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
        public async Task<JsonResult> GetSpotifyAudioFeaturesAsync(string id)
        {
            var features = new JObject();
            var uri = new Uri("https://api.spotify.com/v1/audio-features/" + id);
            JsonResult response = await SendAsyncSpotifyQuery(uri);


            //IEnumerable <JToken> tokens = response.SelectTokens("$.audio_features[*]");
            //foreach (JToken token in tokens)
            //{
            //    features = JsonConvert.DeserializeObject<JObject>(token.ToString());
            //}

            return response;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        private async Task<JsonResult> SendAsyncSpotifyQuery(Uri uri)
        {
            string response = string.Empty;
            using (var client = new WebClient())
            {
                try
                {
                    string token = GetSpotifyAccessToken();
                    client.Headers.Add("Authorization", "Bearer " + token);
                    response = await client.DownloadStringTaskAsync(uri);
                }
                catch (WebException ex)
                {
                    //https://developer.spotify.com/web-api/user-guide/#error-details
                    Console.WriteLine(ex.Message);
                    HttpWebResponse httpResponse = (HttpWebResponse)ex.Response;
                    if (httpResponse.StatusDescription.Contains("Too Many Requests"))
                    {
                        //https://developer.spotify.com/web-api/user-guide/#rate-limiting
                        int seconds = Convert.ToInt32(ex.Response.Headers["Retry-After"]);
                        seconds = (seconds < 5) ? 5 : seconds; //if < 5, wait 5 seconds

                        Console.WriteLine("Will retry after {0} second(s)", seconds);
                        Thread.Sleep(seconds * 1000);
                        Console.WriteLine("Retrying");
                        return await SendAsyncSpotifyQuery(uri);
                    }
                }
            }
            return Json(response);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetSpotifyAccessToken()
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

        #region Misc. Helper Methods
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
