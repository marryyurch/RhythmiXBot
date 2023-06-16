using Amazon.DynamoDBv2.DataModel;

namespace AWS.DynamoDB.Models
{
    [DynamoDBTable("Users")]
    public class User
    {
        [DynamoDBHashKey("id")]  // partion key
        public string ID { get; set; }
        
        [DynamoDBProperty("name")]  // attribute
        public string Name { get; set; }

        [DynamoDBProperty("password")] // attribute 
        public string Password { get; set; }
    }
}
