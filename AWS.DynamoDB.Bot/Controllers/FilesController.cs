using AWS.DynamoDB.Bot.Clients;
using Microsoft.AspNetCore.Mvc;
namespace AWS.DynamoDB.Bot.Controllers;

[Route("api/files")]
[ApiController]
public class FilesController : ControllerBase
{
    private readonly S3Client _s3Client;
    public FilesController(S3Client s3Client)
    {
        _s3Client = s3Client;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFileAsync(IFormFile file, string bucketName, string? prefix)
    {
        if (await _s3Client.UploadFileAsync(file, bucketName, prefix))
            return Ok($"File {prefix}/{file.FileName} uploaded to S3 successfully!");
        return NotFound($"Bucket {bucketName} does not exist.");
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAllFilesAsync(string bucketName, string? prefix)
    {
        var s3Objects = await _s3Client.GetAllFilesAsync(bucketName, prefix);
        return s3Objects is not null ? Ok(s3Objects) : NotFound($"Bucket {bucketName} does not exist.");
    }

    [HttpGet("get-by-key")]
    public async Task<IActionResult> GetFileByKeyAsync(string bucketName, string key)
    {
        var s3Object = await _s3Client.GetFileByKeyAsync(bucketName, key);
        return s3Object is null
            ? File(s3Object.ResponseStream, s3Object.Headers.ContentType)
            : NotFound($"Bucket {bucketName} does not exist.");
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteFileAsync(string bucketName, string key)
    {
        if (await _s3Client.DeleteFileAsync(bucketName, key))
            return NoContent();
        return NotFound($"Bucket {bucketName} does not exist");
    }
}