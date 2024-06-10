using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.DataModel.Args;
using Minio.DataModel.Response;
using POPPER_Server.Dtos;
using POPPER_Server.Models;

namespace POPPER_Server.Services;

public interface IUserProfileService
{
    public Task<bool> SetProfilePicture(User user, FileUploadDto picture);
    public Task<FileContentResult> GetProfilePicture(User user);
}

public class UserProfileService : IUserProfileService
{
    private readonly IMinioClient _minioClient;

    private const string BucketName = "profile-pictures";

    public UserProfileService(IMinioClient minioClient)
    {
        _minioClient = minioClient;
    }

    public async Task<bool> SetProfilePicture(User user, FileUploadDto picture)
    {
        try
        {
            await CreateIfBucketNotExists();
            string filePath = Path.GetTempFileName();
            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await picture.File.CopyToAsync(stream);
            }

            PutObjectArgs putObject = new PutObjectArgs()
                .WithBucket(BucketName)
                .WithObject(user.Guid)
                .WithFileName(filePath)
                .WithContentType(picture.File.ContentType);

            PutObjectResponse result = await _minioClient.PutObjectAsync(putObject).ConfigureAwait(true);
        }
        catch (Exception e)
        {
            //TODO remove on relese
            Console.WriteLine(e);
            return false;
        }

        return true;
    }

    public async Task<FileContentResult> GetProfilePicture(User user)
    {
        //TODO handle file extenson differently
        await CreateIfBucketNotExists();
        StatObjectArgs statPictureArgs = new StatObjectArgs()
            .WithBucket(BucketName)
            .WithObject(user.Guid);
        _ = await _minioClient.StatObjectAsync(statPictureArgs);

        GetObjectArgs getPictureArgs = new GetObjectArgs()
            .WithBucket(BucketName)
            .WithObject(user.Guid)
            .WithCallbackStream(stream =>
            {
                using FileStream fileStream = File.Create(user.Guid);
                stream.CopyTo(fileStream);
                stream.Dispose();
            });

        var ObjData = await _minioClient.GetObjectAsync(getPictureArgs);

        byte[] bytes = await File.ReadAllBytesAsync(user.Guid);
        FileContentResult file = new FileContentResult(bytes, ObjData.ContentType)
        {
            FileDownloadName = $"{user.Guid}.{ObjData.ContentType.Split("/")[1]}",
        };
        File.Delete(user.Guid);
        return file;
    }

    private async Task CreateIfBucketNotExists()
    {
        BucketExistsArgs? bukerArgs = new BucketExistsArgs()
            .WithBucket(BucketName);

        bool bucketExists = await _minioClient.BucketExistsAsync(bukerArgs)
            .ConfigureAwait(false);

        if (!bucketExists)
        {
            MakeBucketArgs newBucket = new MakeBucketArgs()
                .WithBucket(BucketName);
            await _minioClient.MakeBucketAsync(newBucket)
                .ConfigureAwait(true);
        }
    }
}