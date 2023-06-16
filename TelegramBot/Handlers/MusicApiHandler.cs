using SpotifyAPI.Web;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot.Handlers
{
    internal static class MusicApiHandler
    {
        private static HttpClient _musicClient = new()
        {
            BaseAddress = new Uri("https://localhost:7123/")
        };

#if CUSTOM_AWAIT
        public static T Await<T>(Task<T> task)
        {
            task.Wait();
            return task.Result;
        }
#endif
        public static async Task ConnectSpotify(ITelegramBotClient botClient, CallbackQuery? callbackQuery)
        {
            var response = await _musicClient.GetAsync("api/spotify/get-current-user");
            if (response.GetType() != typeof(PrivateUser))
            {
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, "You have to authorize",
                    replyMarkup: new InlineKeyboardMarkup(new[]
                    {
                        InlineKeyboardButton.WithUrl(
                            text: "Authorize me",
                            url: await response.Content.ReadAsStringAsync())
                    }));
            }
            else if (response.GetType() != typeof(string))
            {
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat,
                    "That's it. You connected your Spotify account.");
            }
        }
    }
}
