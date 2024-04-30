using CommunityToolkit.HighPerformance;
using Minio;
using Minio.DataModel.Args;
using POPPER_Server.Dtos;
using POPPER_Server.Models;

namespace POPPER_Server.Services;

interface IUserProfileService
{
    public Task<User> SetProfilePicture(User user, FileUploadDto picture);
}

public class UserProfileService : IUserProfileService
{
    private readonly IMinioClient _minioClient;

    private const string contentType = "application/zip";
    private const string bucketName = "profile-pictures";

    public UserProfileService(IMinioClient minioClient)
    {
        _minioClient = minioClient;
    }

    public async Task<User> SetProfilePicture(User user, FileUploadDto picture)
    {
        BucketExistsArgs? bukerArgs = new BucketExistsArgs()
            .WithBucket(bucketName);

        bool found = await _minioClient.BucketExistsAsync(bukerArgs)
            .ConfigureAwait(false);

        if (!found)
        {
            MakeBucketArgs newBucket = new MakeBucketArgs()
                .WithBucket(bucketName);
            await _minioClient.MakeBucketAsync(newBucket)
                .ConfigureAwait(false);
        }

        ReadOnlyMemory<byte> picBytes = await File.ReadAllBytesAsync(picture.File.FileName)
            .ConfigureAwait(false);

        using var filestream = picBytes.AsStream();
        string filePath = Path.GetTempFileName();

        PutObjectArgs putObject = new PutObjectArgs()
            .WithBucket(bucketName)
            .WithObject(user.Guid)
            .WithFileName(filePath)
            .WithStreamData(filestream)
            .WithContentType(contentType);
        await _minioClient.PutObjectAsync(putObject)
            .ConfigureAwait(false);

        return user;
    }
}