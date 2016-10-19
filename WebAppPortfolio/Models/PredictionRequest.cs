using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAppPortfolio.Models
{
    public class PredictionRequest
    {
        public Dictionary<string, List<Dictionary<string, string>>> Inputs { get; set; }
        public Dictionary<string, string> GlobalParameters { get; set; }

        public PredictionRequest()
        {
            Inputs = new Dictionary<string, List<Dictionary<string, string>>>();
            GlobalParameters = new Dictionary<string, string>();
        }

        public static PredictionRequest CreateRequest(JObject obj, int year)
        {
            var request = new PredictionRequest();
            int decade = (int)Math.Floor((decimal)year / 10) * 10;
            var inputs = new List<Dictionary<string, string>>(){
                new Dictionary<string, string>() {
                {
                    "Decade", decade.ToString()
                },
                {
                    "Danceability", GetTokenValue(obj, "danceability")
                },
                {
                    "Energy", GetTokenValue(obj, "energy")
                },
                {
                    "Loudness", GetTokenValue(obj, "loudness")
                },
                {
                    "Speechiness", GetTokenValue(obj, "speechiness")
                },
                {
                    "Acousticness", GetTokenValue(obj, "acousticness")
                },
                {
                    "Instrumentalness", GetTokenValue(obj, "instrumentalness")
                },
                {
                    "Liveness", GetTokenValue(obj, "liveness")
                },
                {
                    "Valence", GetTokenValue(obj, "valence")
                },
                //{
                //    "Tempo", GetTokenValue(obj, "tempo") //used with the AUC optimized model
                //},
                {
                    "Duration", GetTokenValue(obj, "duration_ms")
                },
            }};

            request.Inputs.Add("input1", inputs);

            return request;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string GetTokenValue(JObject obj, string token)
        {
            string value = "0"; //default value of 0
            var jToken = obj.SelectToken("$." + token);
            if (jToken != null)
            {
                value = jToken.ToString();
            }
            return value;
        }

    }
}