using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.DataModel.Args;
using Minio.DataModel.Response;
using POPPER_Server.Dtos;
using POPPER_Server.Helpers;
using POPPER_Server.Models;

namespace POPPER_Server.Services;

public interface IPostService
{
    public Task<Post> CreatePost(User user, NewPostDto dto);
    public Task UploadMedaToPost(string postGuid, User user, FileUploadDto file);
    public Task<FileContentResult> GetMedia(string guid);
}

public class PostService : IPostService
{
    private readonly IMapper _mapper;
    private readonly PopperdbContext _context;
    private readonly IMinioClient _minioClient;

    private const string BucketName = "posts";

    public PostService(IMapper mapper, PopperdbContext context, IMinioClient minio)
    {
        _mapper = mapper;
        _context = context;
        _minioClient = minio;
    }

    public async Task<Post> CreatePost(User user, NewPostDto dto)
    {
        Post newPost = _mapper.Map<Post>(dto);
        newPost.UserId = user.Id;
        await _context.Posts.AddAsync(newPost);
        await _context.SaveChangesAsync();
        return newPost;
    }

    public async Task UploadMedaToPost(string postGuid, User user, FileUploadDto file)
    {
        await CreateIfBucketNotExists();
        Post post = _context.Posts.FirstOrDefault(p => p.Guid == postGuid);
        if (post == null) throw new Exception("Post not found");
        
        if(post.MediaGuid != null) return;

        post.MediaGuid = Guid.NewGuid().ToString();

        string filePath = Path.GetTempFileName();
        try
        {
            using var stream = new FileStream(filePath, FileMode.Create);
            file.File.CopyTo(stream);
        }
        catch
        {
            File.Delete(filePath);
            throw new Exception("Cant copy the file");
        }

        try
        {
            PutObjectArgs putPostArgs = new PutObjectArgs()
                .WithBucket(BucketName)
                .WithObject(post.MediaGuid)
                .WithFileName(filePath)
                .WithContentType(file.File.ContentType);

            PutObjectResponse result = await _minioClient.PutObjectAsync(putPostArgs).ConfigureAwait(true);
            File.Delete(filePath);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return;
        }
        finally
        {
            await _context.SaveChangesAsync();
        }
    }

    public async Task<FileContentResult> GetMedia(string guid)
    {
        await CreateIfBucketNotExists();
        //TODO check if needed
        StatObjectArgs statPostArgs = new StatObjectArgs()
            .WithBucket(BucketName)
            .WithObject(guid);

        _ = await _minioClient.StatObjectAsync(statPostArgs);

        GetObjectArgs getPostArgs = new GetObjectArgs()
            .WithBucket(BucketName)
            .WithObject(guid)
            .WithCallbackStream(stream =>
            {
                using FileStream fileStream = File.Create(guid);
                stream.CopyTo(fileStream);
                stream.Dispose();
            });

        var ObjData = await _minioClient.GetObjectAsync(getPostArgs);

        byte[] bytes = await File.ReadAllBytesAsync(guid);
        FileContentResult file = new FileContentResult(bytes, ObjData.ContentType)
        {
            FileDownloadName = $"{guid}.{ObjData.ContentType.Split("/")[1]}"
        };
        File.Delete(guid);

        Post post = _context.Posts.FirstOrDefault(p => p.Guid == guid);
        //TODO It needs to return other post data
        PostDto postDto = _mapper.Map<PostDto>(post);

        return file;
    }

    private async Task CreateIfBucketNotExists()
    {
        //TODO check does not upload the first file after creating bucket
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