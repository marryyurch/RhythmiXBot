using SpotifyAPI;
using SpotifyAPI.Web;

namespace TelegramBot.Handlers
{
    internal static class SpotifyUserManager
    {
        private static readonly HttpClient _spotifyClient = new()
        {
            BaseAddress = new Uri("https://localhost:7123/")
        };

        private static async Task GetCurrentUser()
        {
            await _spotifyClient.GetAsync("api/spotify/get-current-user");
        }

        public static async Task<string> GetPlaylists()
        {
            var playlists = await _spotifyClient.GetAsync("api/spotify/get-currret-user-playlists");
            if (!playlists.IsSuccessStatusCode)
                await GetCurrentUser();
            return playlists.Content.ReadAsStringAsync().Result;
        }
    }
}
