using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using S3.Demo.API.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot.Handlers
{
    internal class S3ApiHandler
    {
        private static readonly HttpClient _s3Client = new()
        {
            BaseAddress = new Uri("https://localhost:7065/")
        };
        private static Dictionary<long, Dictionary<string, string>> allMusicFiles = new (); // bot chat id, (song id, song name)

        public static async Task<bool> DownloadAllSongs(ITelegramBotClient botClient, CallbackQuery? callbackQuery)
        {
            var response = await _s3Client.GetAsync($"api/files/get-all?bucketName={callbackQuery.From.FirstName.ToLower() + "-bucket"}&prefix=music");
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                var songs = JsonConvert.DeserializeObject<List<S3Object>>(content);

                //foreach (var song in songs)
                //    await botClient.SendAudioAsync(callbackQuery.Message.Chat, InputFile.FromUri(song.PresignedUrl));
                try
                {
                    foreach (var songId in allMusicFiles[callbackQuery.Message.Chat.Id].Keys)
                        await botClient.SendAudioAsync(callbackQuery.Message.Chat, InputFile.FromFileId(songId));
                    return true;
                }
                catch (Exception)
                {
                    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, "Something went wrong..");
                    return false;
                }
            }

            await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, "Something went wrong..");
            return false;
        }

        public static async Task<bool> UploadUserTrack(ITelegramBotClient botClient, Message message, TelegramBot.UserData user)
        {
            var file = await botClient.GetFileAsync(message.Audio.FileId);
            
            var stream = new FileStream($"music\\{message.Audio.FileName}.mp3", FileMode.Create);

            await botClient.DownloadFileAsync(file.FilePath, stream);
            
            var bucketName = message.From.FirstName.ToLower() + "-bucket";
            var fileFormData = new MultipartFormDataContent {};
            var postFile = new FormFile(stream, 0, stream.Length, "null", $"music\\{message.Audio.FileName}.mp3")
            {
                Headers = new HeaderDictionary(),
                ContentType = "audio/mpeg"
            }; 

            message.Audio.FileName ??= $"my_song{message.Audio.FileId}";
            user.songNames.Add(message.Audio.FileId, message.Audio.FileName);
            allMusicFiles[message.Chat.Id] = user.songNames;

            await _s3Client.PostAsync($"api/buckets/create?bucketName={bucketName}", new StringContent("bucket-name"));

            await _s3Client.PostAsync($"api/files/upload?file={postFile}&bucketName={bucketName}&prefix=music", fileFormData);

            stream.Close();
            
            return true;
        }
    }
}
