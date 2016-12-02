namespace WebAppPortfolio.Models
{
    /// <summary>
    /// Representation of authorization token returned from Spotify
    /// to deserialize from JSON
    /// See https://developer.spotify.com/web-api/authorization-guide/#client-credentials-flow
    /// </summary>
    public class SpotifyToken
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }   
    }
}