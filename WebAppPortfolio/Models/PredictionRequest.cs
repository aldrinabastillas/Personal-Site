using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace WebAppPortfolio.Models
{
    /// <summary>
    /// Input format expected from machine learning web service
    /// See https://docs.microsoft.com/en-us/azure/machine-learning/machine-learning-consume-web-services#request-response-service-rrs
    /// </summary>
    public class PredictionRequest
    {
        #region Public Properties
        public Dictionary<string, List<Dictionary<string, string>>> Inputs { get; set; }
        public Dictionary<string, string> GlobalParameters { get; set; }
        #endregion

        #region Constructor
        public PredictionRequest()
        {
            Inputs = new Dictionary<string, List<Dictionary<string, string>>>();
            GlobalParameters = new Dictionary<string, string>();
        }
        #endregion

        /// <summary>
        /// Parses a JSON response object from Spotify puts it in the expected format
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="year"></param>
        /// <returns></returns>
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
        /// Parses JSON Object for a particular token and returns the token's value
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