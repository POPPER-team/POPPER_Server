using Minio;
using System.IO;
using System.Threading.Tasks;
using Minio.DataModel.Args;
using Minio.Exceptions;

namespace POPPER_Server.Services;

public interface IMinioService
{
    public Task UploadFileAsync(string bucketName, string objectName, string filePath);
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
}