using Amazon.S3;
using Amazon.S3.Model;
using S3Object = S3.Demo.API.Models.S3Object;

namespace AWS.DynamoDB.Bot.Clients
{
    public class S3Client
    {
        private readonly IAmazonS3 _s3Client;
        public S3Client(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }
        public async Task<bool> CreateBucketAsync(string bucketName)
        {
            var bucketExists = await _s3Client.DoesS3BucketExistAsync(bucketName);
            if (bucketExists) return false;
            await _s3Client.PutBucketAsync(bucketName);
            return true;
        }
        
        public async Task<IEnumerable<string>> GetAllBucketAsync()
        {
            var data = await _s3Client.ListBucketsAsync();
            var buckets = data.Buckets.Select(b => b.BucketName);
            return buckets;
        }
        
        public async Task DeleteBucketAsync(string bucketName)
        {
            var listVersionsRequest = new ListVersionsRequest
            {
                BucketName = bucketName,
                MaxKeys = 1000 // max amount of versions which i get by 1 request
            };
            ListVersionsResponse listVersionsResponse;
            do
            {
                listVersionsResponse = await _s3Client.ListVersionsAsync(listVersionsRequest);
                foreach (var version in listVersionsResponse.Versions)
                {
                    await _s3Client.DeleteObjectAsync(bucketName, version.Key, version.VersionId);
                }
                listVersionsRequest.KeyMarker = listVersionsResponse.NextKeyMarker;
                listVersionsRequest.VersionIdMarker = listVersionsResponse.NextVersionIdMarker;
            }
            while (listVersionsResponse.IsTruncated);
            await _s3Client.DeleteBucketAsync(bucketName);
        }
        
        public async Task<bool> UploadFileAsync(IFormFile file, string bucketName, string? prefix)
        {
            var bucketExists = await _s3Client.DoesS3BucketExistAsync(bucketName);
            if (!bucketExists) return false;
            var request = new PutObjectRequest()
            {
                BucketName = bucketName,
                Key = string.IsNullOrEmpty(prefix) ? file.FileName : $"{prefix?.TrimEnd('/')}/{file.FileName}",
                InputStream = file.OpenReadStream()
            };
            request.Metadata.Add("Content-Type", file.ContentType);
            await _s3Client.PutObjectAsync(request);
            return true;
        }
        
        public async Task<IEnumerable<S3Object>?> GetAllFilesAsync(string bucketName, string? prefix)
        {
            var bucketExists = await _s3Client.DoesS3BucketExistAsync(bucketName);
            if (!bucketExists) return null;
            var request = new ListObjectsV2Request()
            {
                BucketName = bucketName,
                Prefix = prefix
            };
            var result = await _s3Client.ListObjectsV2Async(request);
            var s3Objects = result.S3Objects.Select(s =>
            {
                var urlRequest = new GetPreSignedUrlRequest()
                {
                    BucketName = bucketName,
                    Key = s.Key,
                    Expires = DateTime.UtcNow.AddMinutes(1)
                };
                return new S3Object()
                {
                    Name = s.Key.ToString(),
                    PresignedUrl = _s3Client.GetPreSignedURL(urlRequest),
                };
            });
            return s3Objects;
        }

        public async Task<GetObjectResponse?> GetFileByKeyAsync(string bucketName, string key)
        {
            var bucketExists = await _s3Client.DoesS3BucketExistAsync(bucketName);
            if (!bucketExists) return null;
            var s3Object = await _s3Client.GetObjectAsync(bucketName, key);
            return s3Object;
        }

        public async Task<bool> DeleteFileAsync(string bucketName, string key)
        {
            var bucketExists = await _s3Client.DoesS3BucketExistAsync(bucketName);
            if (!bucketExists) return false;
            await _s3Client.DeleteObjectAsync(bucketName, key);
            return true;
        }
    }
}