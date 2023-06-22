using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Handlers;

namespace TelegramBot;
enum UserInputState
{
    None,
    Name,
    Password,
    SongName,
    PlaylistName
}
class UserData
{
    public UserInputState State = UserInputState.None;

    public string Name;
    public string Password;
    public string SongName;
    public string PlaylistName;

    public Dictionary<string, string> songNames = new (); // file_id and file name
    ~UserData()
    {
        
    }

    public async Task HandleCallbackQuery(ITelegramBotClient botClient, CallbackQuery? callbackQuery)
    {
        string[] _callbackQueryParts = callbackQuery.Data.Split('_');
        string _callbackType = _callbackQueryParts[0];

        switch (_callbackType)
        {
            case "creating new user":
            {
                if (State == UserInputState.Password)
                {
                    if (await DbApiHandler.CreateUser(callbackQuery.From.Id, Name, Password) != null) 
                        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, "Success.");
                    else
                        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, "You already have an account");
                    
                }
                else if (State == UserInputState.None)
                {
                    State = UserInputState.Name;
                    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, "Tell me your name");
                }
                break;
            }
            case "signing in":
            {
                if (State == UserInputState.Password)
                {
                    if (await DbApiHandler.SignIn(Name, Password, callbackQuery.From.Id))
                        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, "You successfully signed in.");
                    else
                        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, "I can't do this. Or you already signed in, or you give me the wrong credentials.");
                }
                else if (State == UserInputState.None)
                {
                    State = UserInputState.Name;
                    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, "Your name?");
                }
                break;
            }
            case "deleting account":
            {
                if (await DbApiHandler.DeleteUser(callbackQuery.From.Id))
                {
                    RhythmiXBot.usersData.Remove(callbackQuery.From.Id);
                    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, "Success.");
                }
                else
                    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, "You have to sign in.");

                break;
            }
            case "deleting song":
            {
                if (State != UserInputState.None)
                {
                    if (await S3ApiHandler.DeleteSong(botClient, callbackQuery, SongName))
                        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, "Delete successfully.");
                    //else
                    //    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, "Something went wrong..");
                    break;
                }
                State = UserInputState.SongName;
                break;
            }
            case "searching song":
            {
                if (State != UserInputState.None)
                {
                    if (!await S3ApiHandler.SearchSong(botClient, callbackQuery, SongName))
                        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, "I can't find it..");
                    break;
                }

                State = UserInputState.SongName;
                break;
            }
            case "creating playlist":
            {
                if (State != UserInputState.None)
                {
                    if (await PlaylistManager.CreatePlaylist(botClient, callbackQuery, PlaylistName))
                        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, "Created successfully.");
                    else await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, "This playlist name is already exist.");
                    break;
                }

                State = UserInputState.PlaylistName;
                break;
            }
            case "deleting playlist":
            {
                if (State != UserInputState.None)
                {
                    if (await PlaylistManager.DeletePlaylist(botClient, callbackQuery, PlaylistName))
                        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, "Deleted successfully.");
                    else await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, "This playlist does not exist.");
                    break;
                }

                State = UserInputState.PlaylistName;
                break;
            }
            case "searching playlist":
            {
                if (State != UserInputState.None)
                {
                    if (await PlaylistManager.SearchPlaylist(botClient, callbackQuery, PlaylistName))
                        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, "Found.");
                    else await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, "This playlist does not exist.");
                    break;
                }

                State = UserInputState.PlaylistName;
                break;
            }
            case "adding to playlist":
            {
                if (State == UserInputState.SongName)
                {
                    if (await PlaylistManager.AddToPlaylist(botClient, callbackQuery, PlaylistName, SongName))
                        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, "Added successfully.");
                    else await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, "Something went wrong..");
                }
                if (State == UserInputState.PlaylistName)
                {
                    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, "Enter song name");
                    State = UserInputState.SongName;
                }

                State = UserInputState.PlaylistName;
                break;
            }
        }
    }
}