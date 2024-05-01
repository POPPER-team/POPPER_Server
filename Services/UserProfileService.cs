using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.DataModel.Args;
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
    private const string contentType = "image/jpg";
    private const string bucketName = "profile-pictures";

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
            //TODO handle file extension differently
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
                .ConfigureAwait(true);
        }
        catch (Exception e)
        {
            return false;
        }

        return true;
    }

    public async Task<FileContentResult> GetProfilePicture(User user)
    {
        //TODO handle file extenson differently
        string fileExtension = "jpg";
        string fileName = $"{user.Guid}.{fileExtension}";
        await CreateIfBucketNotExists();
        StatObjectArgs statPictureArgs = new StatObjectArgs()
            .WithBucket(bucketName)
            .WithObject(fileName);
        _ = await _minioClient.StatObjectAsync(statPictureArgs);

        GetObjectArgs getPictureArgs = new GetObjectArgs()
            .WithBucket(bucketName)
            .WithObject(fileName)
            .WithCallbackStream(stream =>
            {
                using var fileStream = File.Create($"{user.Guid}.jpg");
                stream.CopyTo(fileStream);
                stream.Dispose();
            });

        _ = await _minioClient.GetObjectAsync(getPictureArgs)
            .ConfigureAwait(true);

        byte[] bytes = await File.ReadAllBytesAsync(fileName).ConfigureAwait(true);
        FileContentResult file = new FileContentResult(bytes, "image/jpg")
        {
            FileDownloadName = fileName,
        };
        File.Delete(fileName);
        return file;
    }


    private async Task CreateIfBucketNotExists()
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
                .ConfigureAwait(true);
        }
    }
}