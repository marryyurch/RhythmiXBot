using Newtonsoft.Json;
using System.Text;
using MusicAPI.Models;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Http;

namespace MusicAPI.Clients
{
    public class SpotiClient
    {
        private static string _secretClient = "643c092c22174852a6530ec0fc920402";
        private static string _clientID = "3c1d8271e3c84c9ea6018d8b00c436b4";

        private static readonly HttpClient _client = new ();
        private static readonly SpotifyClient _spotifyClient = new (GetAccessToken().Result);

        private static async Task<string> GetAccessToken()
        {
            var base64Credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientID}:{_secretClient}"));

            _client.DefaultRequestHeaders.Add("Authorization", $"Basic {base64Credentials}");
            _client.DefaultRequestHeaders.Add("Accept", "application/json");

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            });
            var response = await _client.PostAsync("https://accounts.spotify.com/api/token", content);

            var responseContent = await response.Content.ReadAsStringAsync();
            var accessToken = JsonConvert.DeserializeObject<AccessToken>(responseContent);
            var token = accessToken.Access_token;

            return token;
        }

        public async Task<FullPlaylist> GetPlaylist(string playlistId)
        {
            var playlist = await _spotifyClient.Playlists.Get(playlistId);
            return playlist;
        }

        public async Task<PrivateUser> GetCurrentUser()
        {
            var user = await _spotifyClient.UserProfile.Current();
            return user;
        }

        public string SendAuthorizationLink()
        {
            string botUrl = "https://t.me/RtmXBot";
            string scopes = "playlist-read-private " +
                            "playlist-read-collaborative " +
                            "playlist-modify-private " +
                            "playlist-modify-public " +
                            "user-top-read " +
                            "user-library-modify " +
                            "user-library-read " +
                            "user-read-email " +
                            "user-read-private";
            return $"https://accounts.spotify.com/authorize?client_id=" +
                   $"{_clientID}&response_type=code&" +
                   $"redirect_uri={botUrl}&" +
                   $"scope={scopes}";
        }
    }
}
