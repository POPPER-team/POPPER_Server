using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.DataModel.Args;
using Minio.DataModel.Response;
using POPPER_Server.Dtos;
using POPPER_Server.Models;

namespace POPPER_Server.Services;

public interface IPostService
{
    public Task CreatePost(User user, NewPostDto dto);
}

public class PostService : IPostService
{
    private readonly IMapper _mapper;
    private readonly PopperdbContext _context;
    private readonly IMinioClient _minio;

    private const string BucketName = "posts";

    public PostService(IMapper mapper, PopperdbContext context, IMinioClient minio)
    {
        _mapper = mapper;
        _context = context;
        _minio = minio;
    }

    public async Task CreatePost(User user, NewPostDto dto)
    {
        Post newPost = _mapper.Map<Post>(dto);

        string filePath = Path.GetTempFileName();
        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await dto.Media.File.CopyToAsync(stream);
        }

        try
        {
            PutObjectArgs putPostArgs = new PutObjectArgs()
                .WithBucket(BucketName)
                .WithObject(newPost.Guid)
                .WithFileName(filePath)
                .WithContentType(dto.Media.File.ContentType);

            PutObjectResponse result = await _minio.PutObjectAsync(putPostArgs).ConfigureAwait(true);
        }
        catch (Exception e)
        {
            return;
        }

        
        
       await _context.Posts.AddAsync(newPost).ConfigureAwait(false);
    }

    public async Task<FileContentResult> GetPost(string guid)
    {
        throw new NotImplementedException();
    }
}