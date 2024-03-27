using Minio;
using System.Reactive.Linq;
using Minio.DataModel.Args;
using Minio.Exceptions;

namespace POPPER_Server.Services;

public interface IMinioService
{
    public Task UploadFileAsync(string bucketName, string objectName, string filePath);

    public Task DownloadFileAsync(string testBucker, string fileName, string filePath);

    Task<IEnumerable<string>> GetListFilesAsync(string testBucker);
}

public class MinioService : IMinioService
{
    private readonly IMinioClient _minioClient;

    public MinioService(IMinioClient minioClient)
    {
        _minioClient = minioClient;
    }

    public async Task UploadFileAsync(string bucketName, string objectName, string filePath)
    {
        var contentType = "application/zip";

        try
        {
            // Make a bucket on the server, if not already present.
            var beArgs = new BucketExistsArgs()
                .WithBucket(bucketName);
            bool found = await _minioClient.BucketExistsAsync(beArgs).ConfigureAwait(false);
            if (!found)
            {
                var mbArgs = new MakeBucketArgs()
                    .WithBucket(bucketName);
                await _minioClient.MakeBucketAsync(mbArgs).ConfigureAwait(false);
            }
            // Upload a file to bucket.
            var putObjectArgs = new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithFileName(filePath)
                .WithContentType(contentType);
            await _minioClient.PutObjectAsync(putObjectArgs).ConfigureAwait(false);
            Console.WriteLine("Successfully uploaded " + objectName);
        }
        catch (MinioException e)
        {
            Console.WriteLine("File Upload Error: {0}", e.Message);
        }
    }

    public async Task DownloadFileAsync(string bucketName, string objectName, string filePath)
    {
        try
        {
            var statObjectArgs = new StatObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName);
            await _minioClient.StatObjectAsync(statObjectArgs);

            var getObjectArgs = new GetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithFile(filePath);
            await _minioClient.GetObjectAsync(getObjectArgs);

            Console.WriteLine($"Successfully downloaded {objectName} to {filePath}");
        }
        catch (MinioException e)
        {
            Console.WriteLine($"File Download Error: {e.Message}");
        }
    }

    public Task<IEnumerable<string>> GetListFilesAsync(string bucketName)
    {
        try
        {
            ListObjectsArgs args = new ListObjectsArgs()
                .WithBucket(bucketName);
            return Task.FromResult(_minioClient.ListObjectsAsync(args).Select(i => i.Key).ToEnumerable());
        }
        catch (MinioException e)
        {
            Console.WriteLine($"Error Listing Objects: {e.Message}");
        }
        return Task.FromResult<IEnumerable<string>>(new List<string>());
    }
}