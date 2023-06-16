using AWS.DynamoDB.Bot.Clients;
using Microsoft.AspNetCore.Mvc;

namespace AWS.DynamoDB.Bot.Controllers;

[Route("api/buckets")]
[ApiController]
public class BucketsController : ControllerBase
{
    private readonly S3Client _s3Client;
    public BucketsController(S3Client s3Client)
    {
        _s3Client = s3Client;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateBucketAsync(string bucketName)
    {
        if (await _s3Client.CreateBucketAsync(bucketName))
            return Ok($"Bucket {bucketName} created.");
        return BadRequest($"Bucket {bucketName} already exists.");
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAllBucketAsync()
    {
        var buckets = await _s3Client.GetAllBucketAsync();
        return Ok(buckets);
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteBucketAsync(string bucketName)
    {
        await _s3Client.DeleteBucketAsync(bucketName);
        return NoContent();
    }
}

