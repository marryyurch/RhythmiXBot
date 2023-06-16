namespace MusicAPI.Models
{
    public class AccessToken
    {
        public string Access_token { get; set; }
        private static string Token_type { get; set; }
        private static int Expires_in { get; set; }
    }
}
