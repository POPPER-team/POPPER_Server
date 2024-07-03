using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public Post GetPost(string guid);
    public Task<List<Post>> GetPosts(User user);
    public Task DeletePost(string guid, User user);
    public Task<List<Post>> GetFavoritePosts(User user);
    public Task<List<Post>> GetUserPosts(string guid);
    public void ViewPost(string guid, User user);
}

public class PostService : IPostService
{
    private readonly IMapper _mapper;
    private readonly PopperdbContext _context;
    private readonly IMinioClient _minioClient;

    const string BucketName = "posts";

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
        return await _context.Posts.Include(p => p.User).FirstOrDefaultAsync(p => p.Guid == newPost.Guid);
    }

    public async Task UploadMedaToPost(string postGuid, User user, FileUploadDto file)
    {
        //TODO it does not seem to work
        Post post = _context.Posts.FirstOrDefault(p => p.Guid == postGuid);
        if (post == null)
            throw new Exception("Post not found");

        if (post.MediaGuid != null)
            return;

        post.MediaGuid = Guid.NewGuid().ToString();

        _context.SaveChanges();
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

            PutObjectResponse result = await _minioClient
                .PutObjectAsync(putPostArgs)
                .ConfigureAwait(true);
            File.Delete(filePath);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return;
        }
    }

    public async Task<FileContentResult> GetMedia(string guid)
    {
        //TODO check if needed
        StatObjectArgs statPostArgs = new StatObjectArgs().WithBucket(BucketName).WithObject(guid);

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

    public Post GetPost(string guid)
    {
        Post post = _context
            .Posts.AsNoTracking()
            .Include(p => p.Comments)
            .Include(p => p.Likes)
            .FirstOrDefault(p => p.Guid == guid);
        if (post == null)
            throw new Exception("not found post");
        return post;
    }

    public async Task<List<Post>> GetPosts(User user)
    {
        var Posts = await _context
            .Posts.AsNoTracking()
            .Where(u => !u.Views.Any(v => v.UserId == user.Id))
            .OrderBy(p => p.Views.Count)
            .ThenBy(p => p.Created)
            .Take(10)
            .Include(p => p.Views)
            .Include(p => p.Likes)
            .Include(p => p.Comments)
            .ToListAsync();

        var Followers = await _context
            .Followings.Where(u => u.User.Id == user.Id)
            .Select(u => u.FollowingNavigation)
            .ToListAsync();

        var followerPosts =  _context
            .Posts
            .OrderBy(p => p.Views.Count)
            .ThenBy(p => p.Created)
            .Take(10)
            .Include(p => p.Views)
            .Include(p => p.Likes)
            .Include(p => p.Comments)
            .ToList()
            .Where(p => Followers.Any(u => u.Id == p.UserId))
            .ToList();

        return followerPosts.Union(Posts).ToList();
    }

    public async Task<List<Post>> GetFavoritePosts(User user)
    {
        List<Post> favoritePosts = await _context
            .Saveds.AsNoTracking()
            .Where(s => s.User.Id == user.Id)
            .Include(s => s.Post)
            .ThenInclude(p => p.Saveds)
            .Include(p => p.Post)
            .ThenInclude(p => p.Comments)
            .Include(p => p.Post)
            .ThenInclude(p => p.Likes)
            .Select(s => s.Post)
            .OrderBy(p => p.Created)
            .ToListAsync();
        return favoritePosts;
    }

    public async Task<List<Post>> GetUserPosts(string guid)
    {
        List<Post> userPosts = await _context
            .Posts.AsNoTracking()
            .Include(p => p.Likes)
            .Include(p => p.Comments)
            .Include(p => p.Saveds)
            .Where(p => p.User.Guid == guid)
            .OrderBy(p => p.Created)
            .ToListAsync();
        return userPosts;
    }

    public async Task DeletePost(string guid, User user)
    {
        Post post = await _context.Posts.FirstOrDefaultAsync(p => p.Guid == guid);

        if (post == null)
            throw new Exception("Post not found");
        if (user.Id != post.UserId)
            throw new Exception("You can only delete your post");
        if (post.MediaGuid != null)
        {
            var removeArgs = new RemoveObjectArgs()
                .WithBucket(BucketName)
                .WithObject(post.MediaGuid);
            await _minioClient.RemoveObjectAsync(removeArgs);
        }
        _context.Remove(post);
        await _context.SaveChangesAsync();
    }

    public async void ViewPost(string guid, User user)
    {
        var post = await _context.Posts.FirstOrDefaultAsync(p => p.MediaGuid == guid);

        if (post == null)
            return;
        var view = new View() { PostId = post.Id, UserId = user.Id };
        await _context.Views.AddAsync(view);
        await _context.SaveChangesAsync();
    }
}
