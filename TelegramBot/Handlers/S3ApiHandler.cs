
namespace TelegramBot.Handlers
{
    internal class S3ApiHandler
    {
        private static HttpClient _s3Client = new()
        {
            BaseAddress = new Uri("https://localhost:7065/")
        };

        public static async Task<bool> UploadUserTrack(FileStream s)
        {
            var file = new FileInfo(s.Name); // Replace with the actual file path

            var formData = new MultipartFormDataContent
            {
                { new StreamContent(file.OpenRead()), "file", file.Name },
                { new StringContent("your-bucket-name"), "bucketName" },
                { new StringContent("music"), "prefix" }
            };

            await _s3Client.PostAsync("api/files/upload", formData);

            return true;
        }
    }
}
