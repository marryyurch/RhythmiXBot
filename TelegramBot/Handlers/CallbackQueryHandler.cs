using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot.Handlers
{
    internal static class CallbackQueryHandler
    {
        private static string[] _callbackQueryParts = new []{""};
        private static string _callbackType = "";

        public static async Task HandleUserCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery? callbackQuery)
        {
            _callbackQueryParts = callbackQuery.Data.Split('_');
            _callbackType = _callbackQueryParts[0];

            if (IsQueryOutdated(callbackQuery))
                return; // в єтом случае пользователь жмякнул на кнопку очень давно и
                        // єтот квери он уже ну типа невалидній так шо скип

            switch (_callbackType)
            {
                case "creating new user":
                {
                    if (!DbApiHandler.IsUserAlreadyExist(callbackQuery.From.Id).Result)
                        await RhythmiXBot.usersData[callbackQuery.From.Id].HandleCallbackQuery(botClient, callbackQuery); // to get user data and send to db
                    else 
                        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, "User already created.");
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                    break;
                }
                case "signing in":
                    {
                    if (await DbApiHandler.IsSignedIn(callbackQuery.From.Id))
                        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, "You already signed in.");
                    else
                        await RhythmiXBot.usersData[callbackQuery.From.Id].HandleCallbackQuery(botClient, callbackQuery);
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                    break;
                }
                case "connecting Spoti":
                {
                    await MusicApiHandler.ConnectSpotify(botClient, callbackQuery);
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id); // єта хрень убирает часики, их очень нужно убрать,
                                                                                // потому что если на кнопке есть часики,
                                                                                // юзер не может на нее нажимать =(
                    break;
                }
                case "signing out":
                {
                    if (await DbApiHandler.SignOut(callbackQuery.From.Id))
                        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, "You successfuly signed out.");
                    else
                        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, "I can't sign you out. There can be some factors:\n- You already sign out\n- You have to create a profile first");
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                    break;
                }
                case "deleting account":
                {
                    await RhythmiXBot.usersData[callbackQuery.From.Id].HandleCallbackQuery(botClient, callbackQuery);
                    RhythmiXBot.usersData.Remove(callbackQuery.From.Id);
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                    break;
                }
            }
        }

        public static async Task HandleLibraryOpsCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery? callbackQuery)
        {
            _callbackQueryParts = callbackQuery.Data.Split('_');
            _callbackType = _callbackQueryParts[0];

            if (IsQueryOutdated(callbackQuery))
                return; 

            switch (_callbackType)
            {
                case "dealing with songs":
                {
                    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, "Songs. Choose your option.", replyMarkup: new InlineKeyboardMarkup(
                        new List<List<InlineKeyboardButton>>()
                        {
                            new() {InlineKeyboardButton.WithCallbackData("Add", "adding song"), InlineKeyboardButton.WithCallbackData("Show all", "showing all songs")},
                            new() {InlineKeyboardButton.WithCallbackData("Delete", "deleting song"), InlineKeyboardButton.WithCallbackData("Delete all", "deleting all songs")},
                            new() {InlineKeyboardButton.WithCallbackData("Search","searching song")}
                        })
                    );
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                    break;
                }
                case "dealing with playlists":
                {
                    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, "Playlists. Choose your option.", replyMarkup: new InlineKeyboardMarkup(
                        new List<List<InlineKeyboardButton>>()
                        {
                            new() {InlineKeyboardButton.WithCallbackData("Create", "creating playlist"), InlineKeyboardButton.WithCallbackData("Show all", "showing all playlists")},
                            new() {InlineKeyboardButton.WithCallbackData("Delete", "deleting playlist"), InlineKeyboardButton.WithCallbackData("Delete all", "deleting all playlists")},
                            new() {InlineKeyboardButton.WithCallbackData("Search","searching playlist")}
                        })
                    );
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                    break;
                }
            }
        }

        public static async Task HandleSpotiOpsCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery? callbackQuery)
        {
            _callbackQueryParts = callbackQuery.Data.Split('_');
            _callbackType = _callbackQueryParts[0];

            if (IsQueryOutdated(callbackQuery))
                return;

            switch (_callbackType)
            {
                case "getting all spoty users' playlists":
                {
                    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat,
                        await SpotifyUserManager.GetPlaylists());
                    break;
                }
                case "getting someone's playlists":
                {

                    break;
                }
            }
        }

        public static async Task HandleMusicOpsCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery? callbackQuery)
        {
            _callbackQueryParts = callbackQuery.Data.Split('_');
            _callbackType = _callbackQueryParts[0];

            if (IsQueryOutdated(callbackQuery))
                return;

            switch (_callbackType)
            {
                case "adding song":
                {
                    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, "Load here your mp3 file");
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                    break;
                }
                case "showing all songs":
                {
                    if (!await S3ApiHandler.IsLibraryEmpty(botClient, callbackQuery))
                    {
                        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, "Here's all your songs");
                        await S3ApiHandler.DownloadAllSongs(botClient, callbackQuery);
                    }
                    else await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, "Your library is empty.");
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                    break;
                }
                case "deleting song":
                {
                    if (!await S3ApiHandler.IsLibraryEmpty(botClient, callbackQuery))
                    {
                        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, "Enter song name");
                        await RhythmiXBot.usersData[callbackQuery.From.Id].HandleCallbackQuery(botClient, callbackQuery);
                    }
                    else await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, "Your library is empty.");
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                    break;
                }
                case "deleting all songs":
                {
                    if (!await S3ApiHandler.IsLibraryEmpty(botClient, callbackQuery))
                    {
                        await S3ApiHandler.DeleteAllSongs(botClient, callbackQuery);
                        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, "All songs deleted successfully.");
                    }
                    else await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, "Your library is already empty.");
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                    break;
                }
                case "searching song":
                {
                    if (!await S3ApiHandler.IsLibraryEmpty(botClient, callbackQuery))
                    {
                        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, "Enter song name");
                        await RhythmiXBot.usersData[callbackQuery.From.Id].HandleCallbackQuery(botClient, callbackQuery);
                    }
                    else await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, "Your library is empty.");
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                    break;
                }
                    
                case "creating playlist":
                {

                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                    break;
                }
                case "showing all playlists":
                {

                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                    break;
                }
                case "deleting playlist":
                {

                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                    break;
                }
                case "deleting all playlists":
                {

                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                    break;
                }
                case "searching playlist":
                {

                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                    break;
                }
            }
        }
        private static bool IsQueryOutdated(CallbackQuery? query)
        {
            return query?.Message is null;
        }
    }
}
