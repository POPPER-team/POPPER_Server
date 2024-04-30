using CommunityToolkit.HighPerformance;
using Minio;
using Minio.DataModel.Args;
using POPPER_Server.Dtos;
using POPPER_Server.Models;

namespace POPPER_Server.Services;

public interface IUserProfileService
{
    public Task<User> SetProfilePicture(User user, FileUploadDto picture);
    public Task<string> GetProfilePicture(User user);
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
        if (!await CheckIfBucketExists()) return user;
        string filePath = Path.GetTempFileName();
        string fileExtension = picture.File.FileName.Split(".")[1];
        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await picture.File.CopyToAsync(stream);
        }

        PutObjectArgs putObject = new PutObjectArgs()
            .WithBucket(bucketName)
            .WithObject($"{user.Guid}.{fileExtension}")
            .WithFileName(filePath)
            .WithContentType(contentType);

        _ = await _minioClient.PutObjectAsync(putObject)
            .ConfigureAwait(false);

        return user;
    }

    public async Task<string> GetProfilePicture(User user)
    {
        string fileExtension = "jpg";
        if (!await CheckIfBucketExists()) return null;
        var statPictureArgs = new StatObjectArgs()
            .WithBucket(bucketName)
            .WithObject($"{user.Guid}.{fileExtension}");
        _ = await _minioClient.StatObjectAsync(statPictureArgs);

        var getPictureArgs = new GetObjectArgs()
            .WithBucket(bucketName)
            .WithObject($"{user.Guid}.{fileExtension}")
            .WithCallbackStream(async stream =>
            {
                //TODO save picture to /temp folder
                //TODO .jpg change to be actual extension
                var fileStream = File.Create($"{user.Guid}.jpg");
                await stream.CopyToAsync(fileStream)
                    .ConfigureAwait(false);
                fileStream.DisposeAsync()
                    .ConfigureAwait(false);
                stream.DisposeAsync();
            });

        var objStat = await _minioClient.GetObjectAsync(getPictureArgs)
            .ConfigureAwait(false);

        return $"{user.Guid}.{fileExtension}";
    }


    async Task<bool> CheckIfBucketExists()
    {
        BucketExistsArgs? bukerArgs = new BucketExistsArgs()
            .WithBucket(bucketName);

        bool b = await _minioClient.BucketExistsAsync(bukerArgs)
            .ConfigureAwait(false);

        if (!b)
        {
            MakeBucketArgs newBucket = new MakeBucketArgs()
                .WithBucket(bucketName);
            await _minioClient.MakeBucketAsync(newBucket)
                .ConfigureAwait(false);
        }

        return b;
    }
}