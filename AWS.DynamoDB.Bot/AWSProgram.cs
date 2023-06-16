using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using Amazon.S3;
using AWS.DynamoDB.Bot.Clients;

namespace AWS.DynamoDB.Bot
{
    public class AWSProgram
    {
        public static void Main()
        {
            var builder = WebApplication.CreateBuilder();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            
            var awsOptions = builder.Configuration.GetAWSOptions();
            builder.Services.AddDefaultAWSOptions(awsOptions);
            builder.Services.AddAWSService<IAmazonDynamoDB>();
            builder.Services.AddAWSService<IAmazonS3>();
            builder.Services.AddScoped<IDynamoDBContext, DynamoDBContext>();
            builder.Services.AddScoped<DbClient>();
            builder.Services.AddScoped<S3Client>();

            var app = builder.Build();
            
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}