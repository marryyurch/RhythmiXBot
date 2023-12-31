﻿using Telegram.Bot;
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
            if (await DbApiHandler.IsSignedIn(message.From.Id))
            {
                await botClient.SendTextMessageAsync(message.Chat, "Here's your library. You can deal with...",
                    replyMarkup: new InlineKeyboardMarkup(
                        new List<InlineKeyboardButton>()
                        {
                            InlineKeyboardButton.WithCallbackData("Songs", "dealing with songs"),
                            InlineKeyboardButton.WithCallbackData("Playlists", "dealing with playlists"),
                        })
                );
            }
            else
                await botClient.SendTextMessageAsync(message.Chat, "You have to create profile or sign in first.");
        }
        public static async Task SpotifyCommand(ITelegramBotClient botClient, Message message)
        {
            await botClient.SendTextMessageAsync(message.Chat, "Here's your Spoti prifile. You can...", replyMarkup: new InlineKeyboardMarkup(
                new List<InlineKeyboardButton>()
                {
                    InlineKeyboardButton.WithCallbackData("Get all my playlists", "getting all spoty users' playlists"),
                    InlineKeyboardButton.WithCallbackData("Get someone's playlists", "getting someone's playlists"),
                }));
        }
        public static async Task HelpCommand(ITelegramBotClient botClient, Message message)
        {
            await botClient.SendTextMessageAsync(message.Chat, "What you can ?\n• Adding music files to your personal music library\r\n• Removing these files\r\n• Adding collections for these files (playlists)\r\n• Removing these collections\r\n• Searching for specific songs or playlists\r\n• Adding music files to collections\r\n• Connecting the user to Spotify");
        }
    }
}
