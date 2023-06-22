using System.Net.Http.Json;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using User = TelegramBot.Models.User;

namespace TelegramBot.Handlers
{
    internal static class DbApiHandler
    {
        private static HttpClient _dbClient = new()
        {
            BaseAddress = new Uri("https://localhost:7065/")
        };
        public static Dictionary<long, string> userIds = new(); // bot user and dB Id  
        private static Dictionary<long, string> unsignedUserIds = new();

        public static async Task<User?> CreateUser(long id, string name, string password)
        {
            if (IsUserAlreadyExist(id).Result)
                return null;
            var newUser = new User
            {
                ID = Guid.NewGuid().ToString(),
                Name = name,
                Password = password
            };
            var request = await _dbClient.PostAsJsonAsync("api/Users/create-user", newUser);
            while (!request.IsSuccessStatusCode)
            {
                newUser.ID = Guid.NewGuid().ToString();
                await _dbClient.PostAsJsonAsync("api/Users/create-user", newUser);
            }
            userIds.Add(id, newUser.ID);
            return newUser;
        }

        public static async Task<bool> IsUserAlreadyExist(long id)
        {
            return userIds.ContainsKey(id) || unsignedUserIds.ContainsKey(id);
        }

        public static async Task<bool> SignIn(string name, string password, long id)
        {
            if (unsignedUserIds.ContainsKey(id))
            {
                var response = await _dbClient.GetAsync($"api/Users/get-by-id?userId={unsignedUserIds[id]}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<User>(content);
                    if (user.Name == name && user.Password == password)
                    {
                        unsignedUserIds.Remove(id);
                        userIds[id] = user.ID;
                        return true;
                    }
                }
            }
            return false;
        }

        public static async Task<bool> SignOut(long id)
        {
            if (unsignedUserIds.ContainsKey(id)) return false;
            if (userIds.ContainsKey(id))
            {
                unsignedUserIds.Add(id, userIds[id]);
                userIds[id] = null; 
                return true;
            }

            return false;
        }

        public static async Task<bool> IsSignedIn(long id)
        {
            if (!unsignedUserIds.ContainsKey(id) && userIds.ContainsKey(id))
                return true;
            return false;
        }

        public static async Task<bool> DeleteUser(long id)
        {
            if (userIds.ContainsKey(id) && await IsSignedIn(id))
            {
                await _dbClient.DeleteAsync($"api/Users/delete-by-id?userId={userIds[id]}");
                userIds.Remove(id);
                return true;
            }
            return false;
        }
    }
}
