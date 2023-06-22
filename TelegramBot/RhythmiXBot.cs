using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Polling;
using System.Diagnostics.CodeAnalysis;
using Telegram.Bot.Types.Enums;
using Newtonsoft.Json.Linq;
using TelegramBot.Handlers;

namespace TelegramBot
{
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    internal class RhythmiXBot
    { 
        private static ITelegramBotClient _bot = new TelegramBotClient("6176276102:AAEB69OIlMAhy9O8ULRMA-1MxkzB6EYwHzQ");
        public static Dictionary<long, UserData> usersData = new(); // bot user id and user data
        public static Update previousUpdate = new();

        public static Dictionary<long, long> callbackMessageUserIds = new(); // callback user id and Message user id 

        public static void Start()
        {
            Console.WriteLine("Запущен бот " + _bot.GetMeAsync().Result.FirstName);

            var cts = new CancellationTokenSource();

            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
            };
            _bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
            Console.ReadLine();
        }

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            Console.WriteLine(FormatJson(Newtonsoft.Json.JsonConvert.SerializeObject(update)));
            
            UserData UserData;

            if (update.Type == UpdateType.Message)
            {
                usersData.TryAdd(update.Message.From.Id, new UserData());
                UserData = usersData[update.Message.From.Id];

                switch (UserData.State)
                {
                    case UserInputState.Name:
                        UserData.Name = update.Message.Text;
                        await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Enter password..");
                        UserData.State = UserInputState.Password;
                        return;

                    case UserInputState.Password:
                        UserData.Password = update.Message.Text;
                        await UserData.HandleCallbackQuery(botClient, previousUpdate.CallbackQuery);
                        UserData.State = UserInputState.None;
                        return;

                    case UserInputState.SongName:
                        UserData.SongName = update.Message.Text;
                        await UserData.HandleCallbackQuery(botClient, previousUpdate.CallbackQuery);
                        UserData.State = UserInputState.None;
                        return;
                }

                var message = update.Message;
                if (message.Type == MessageType.Audio)
                {
                    await S3ApiHandler.UploadUserTrack(botClient, message, UserData);
                }
                else
                {
                    switch (message.Text.ToLower())
                    {
                        case "/start":
                            await MenuCommandsHandler.StartCommand(botClient, message);
                            return;
                        case "/user":
                            await MenuCommandsHandler.UserCommand(botClient, message);
                            return;
                        case "/my_library":
                            await MenuCommandsHandler.MyLibraryCommand(botClient, message);
                            return;
                        case "/spotify":
                            await MenuCommandsHandler.SpotifyCommand(botClient, message);
                            return;
                        case "/help":
                            await MenuCommandsHandler.HelpCommand(botClient, message);
                            return;
                        default:
                            await botClient.SendTextMessageAsync(message.Chat, "Can't detect your command.");
                            break;
                    }
                }
                
            }

            else if (update.Type == UpdateType.CallbackQuery)
            {
                usersData.TryAdd(update.CallbackQuery.From.Id, new UserData());
                UserData = usersData[update.CallbackQuery.From.Id];

                switch (UserData.State)
                {
                    case UserInputState.Name:
                        UserData.Name = update.Message.Text;
                        await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Enter password..");
                        UserData.State = UserInputState.Password;
                        return;

                    case UserInputState.Password:
                        UserData.Password = update.Message.Text;
                        await UserData.HandleCallbackQuery(botClient, previousUpdate.CallbackQuery);
                        UserData.State = UserInputState.None;
                        return;
                    case UserInputState.SongName:
                        UserData.SongName = update.Message.Text;
                        await UserData.HandleCallbackQuery(botClient, previousUpdate.CallbackQuery);
                        UserData.State = UserInputState.None;
                        return;
                }

                var callbackQuery = update.CallbackQuery;
                await HandleCallbackQueryAsync(botClient, callbackQuery);
                previousUpdate = update;
            }
        }
        private static async Task HandleCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery? callbackQuery)
        {
            await CallbackQueryHandler.HandleUserCallbackQueryAsync(botClient, callbackQuery);
            await CallbackQueryHandler.HandleLibraryOpsCallbackQueryAsync(botClient, callbackQuery);
            await CallbackQueryHandler.HandleSpotiOpsCallbackQueryAsync(botClient, callbackQuery);
            await CallbackQueryHandler.HandleMusicOpsCallbackQueryAsync(botClient, callbackQuery);
        }
        public static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var formaterJson = FormatJson(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
            Console.WriteLine(formaterJson);
            return Task.CompletedTask;
        }
        public static string FormatJson(string json)
        {
            return JToken.Parse(json).ToString(Newtonsoft.Json.Formatting.Indented);
        }
    }
}