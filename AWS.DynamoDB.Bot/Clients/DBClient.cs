using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using AWS.DynamoDB.Models;

namespace AWS.DynamoDB.Bot.Clients
{
    public class DbClient
    {
        private readonly IDynamoDBContext _context;
        public DbClient(IDynamoDBContext dynamoDbContext)
        {
            _context = dynamoDbContext;
        }
        
        public async Task<User?> GetById(string userId)
        {
            return await _context.LoadAsync<User>(userId);
        }
        
        public async Task<List<User>> GetAllUsers()
        {
            var user = await _context.ScanAsync<User>(default).GetRemainingAsync();
            return user;
        }
        
        public async Task<bool> CreateUser(User userRequest)
        {
            var user = await _context.LoadAsync<User>(userRequest.ID);
            if (user != null) return false; 
            await _context.SaveAsync(userRequest);
            return true;
        }
        
        public async Task<bool> DeleteUser(string userId)
        {
            var user = await _context.LoadAsync<User>(userId);
            if (user == null) return false;
            await _context.DeleteAsync(user);
            return true;
        }

        public async Task<bool> UpdateUser(User userRequest)
        {
            var user = await _context.LoadAsync<User>(userRequest.ID);
            if (user == null) return false;
            //userRequest.ID = user.ID;
            await _context.SaveAsync(userRequest);
            return true;
        }
    }
}
