using Microsoft.AspNetCore.Mvc;
using MusicAPI.Clients;
using MusicAPI.Models;
using SpotifyAPI.Web;

namespace MusicAPI.Controllers
{
    [Route("api/spotify")]
    [ApiController]
    public class SpotifyController : ControllerBase
    {
        private readonly SpotiClient _musicClient;
        public SpotifyController(SpotiClient musicClient)
        {
            _musicClient = musicClient;
        }

        [HttpGet("get-playlist")]
        public async Task<IActionResult> GetUserPlaylist(string playlistId)
        {
            var playlist = await _musicClient.GetPlaylist(playlistId);
            return Ok(playlist);
        }

        [HttpGet("get-current-user")]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var user = await _musicClient.GetCurrentUser();
                return Ok(user);
            }
            catch (APIUnauthorizedException)
            {
                string authorizationLink = _musicClient.SendAuthorizationLink();
                return Ok(authorizationLink);
            }
        }

        [HttpGet("get-currret-user-playlists")]
        public async Task<IActionResult> GetCurUserPlaylists()
        {
            var playlists = await _musicClient.GetCurrentUserPlaylistList();
            return Ok(playlists);
        }


    }
}