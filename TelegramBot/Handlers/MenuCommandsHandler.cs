using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot.Handlers
{
    internal static class MenuCommandsHandler
    {
        public static async Task StartCommand(ITelegramBotClient botClient, Message message)
        {
            await botClient.SendTextMessageAsync(message.Chat, "Hi! You have to create user profile.\n" +
                                                               "If you have one just sign in. Use /user for that.");
        }
        public static async Task UserCommand(ITelegramBotClient botClient, Message message)
        {
            await botClient.SendTextMessageAsync(message.Chat, "Choose your way...", replyMarkup: new InlineKeyboardMarkup(
                new List<List<InlineKeyboardButton>>()
                {
                    new() {InlineKeyboardButton.WithCallbackData("Create new user", "creating new user"), InlineKeyboardButton.WithCallbackData("Sign in", "signing in")},
                    new() {InlineKeyboardButton.WithCallbackData("Spotify", "connecting Spoti") },
                    new() {InlineKeyboardButton.WithCallbackData("Sign out","signing out"), InlineKeyboardButton.WithCallbackData("Delete account", "deleting account") }
                })
            );
        }
        public static async Task MyLibraryCommand(ITelegramBotClient botClient, Message message)
        {
            await botClient.SendTextMessageAsync(message.Chat, "Here's your library. You can deal with...", replyMarkup: new InlineKeyboardMarkup(
                new List<InlineKeyboardButton>()
                {
                    InlineKeyboardButton.WithCallbackData("Songs", "dealing with songs"), 
                    InlineKeyboardButton.WithCallbackData("Playlists", "dealing with playlists"),
                })
            );
        }
        public static async Task SpotifyCommand(ITelegramBotClient botClient, Message message)
        {

        }
        public static async Task HelpCommand(ITelegramBotClient botClient, Message message)
        {
            await botClient.SendTextMessageAsync(message.Chat, "Help text");
        }
    }
}
