
using System.Net.Sockets;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Handlers;

namespace TelegramBot
{
    internal static class PlaylistManager
    {
        private static Dictionary<long, Dictionary<string, List<string>>> playlists = new(); 
                           // bot chat id and (playlist name, list of file ids(songs totally))

        public static async Task<bool> CreatePlaylist(ITelegramBotClient botClient, CallbackQuery? callbackQuery, string playlistName)
        {
            playlists.TryAdd(callbackQuery.Message.Chat.Id, new Dictionary<string, List<string>>());
            if (playlists[callbackQuery.Message.Chat.Id].ContainsKey(playlistName))
                return false;
            playlists[callbackQuery.Message.Chat.Id].Add(playlistName, new List<string>());
            return true;
        }

        public static async Task ShowAllPlaylists(ITelegramBotClient botClient, CallbackQuery? callbackQuery)
        {
            await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, "All your playlists:");
            foreach (var playlistName in playlists[callbackQuery.Message.Chat.Id].Keys)
            {
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, playlistName);
                //await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, "All your playlists:", replyMarkup: new InlineKeyboardMarkup(
                //    new List<InlineKeyboardButton>()
                //    {
                //        InlineKeyboardButton.WithCallbackData(playlistName, "playlist button"),
                //    })
                //);
            }
        }

        public static async Task<bool> IsPlaylistContainerEmpty(ITelegramBotClient botClient, CallbackQuery? callbackQuery)
        {
            if (playlists.ContainsKey(callbackQuery.Message.Chat.Id) && playlists[callbackQuery.Message.Chat.Id].Count > 0)
                return false;
            return true;
        }

        public static async Task<bool> DeletePlaylist(ITelegramBotClient botClient, CallbackQuery? callbackQuery, string playlistName)
        {
            if (!playlists[callbackQuery.Message.Chat.Id].ContainsKey(playlistName))
                return false;
            playlists[callbackQuery.Message.Chat.Id].Remove(playlistName);
            return true;
        }

        public static async Task<bool> DeleteAllPlaylist(ITelegramBotClient botClient, CallbackQuery? callbackQuery)
        {
            playlists.Remove(callbackQuery.Message.Chat.Id);
            return true;
        }

        public static async Task<bool> SearchPlaylist(ITelegramBotClient botClient, CallbackQuery? callbackQuery, string playlistName)
        {
            if (!playlists[callbackQuery.Message.Chat.Id].ContainsKey(playlistName))
                return false;
            await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, $"Playlist: {playlistName}");
            if (playlists[callbackQuery.Message.Chat.Id][playlistName].Count > 0)
            {
                foreach (var song_id in playlists[callbackQuery.Message.Chat.Id][playlistName])
                    await botClient.SendAudioAsync(callbackQuery.Message.Chat, InputFile.FromFileId(song_id));
            }
            else await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, $"This playlist is empty.");
            return true;
        }

        public static async Task<bool> AddToPlaylist(ITelegramBotClient botClient, CallbackQuery? callbackQuery, string playlistName, string songName)
        {
            if (playlists[callbackQuery.Message.Chat.Id].ContainsKey(playlistName))
            {
                if (S3ApiHandler.allMusicFiles[callbackQuery.Message.Chat.Id].ContainsValue(songName))
                {
                    string neededSongId = S3ApiHandler.allMusicFiles[callbackQuery.Message.Chat.Id].FirstOrDefault(x => x.Value == songName).Key;
                }
                else await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, $"This song does not exist.");
            }
            await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, $"This playlist does not exist.");
            return false;
        }
    }
}
