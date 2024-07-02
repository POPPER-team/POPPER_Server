using AutoMapper;
using Microsoft.EntityFrameworkCore;
using POPPER_Server.Models;

public interface IPostActions
{
    public Task<int> LikePost(string guid, User user);
    public Task<List<Comment>> PostComment(string guid, NewCommentDto commentDto);
    public Task<int> SavePost(string guid, User user);
    public Task<string> GetShareLink(string guid);
}

public class PostActions : IPostActions
{
    private readonly PopperdbContext _context;
    private readonly IMapper _mapper;

    public PostActions(PopperdbContext context, IMapper mapper)
    {
        _mapper = mapper;
        _context = context;
    }

    public Task<string> GetShareLink(string guid)
    {
        //        return "https://localhost:5029/Post/get";
        throw new NotImplementedException();
    }

    public async Task<int> LikePost(string guid, User user)
    {
        //TODO check if works
        var post = await _context
            .Posts.Include(p => p.Likes)
            .FirstOrDefaultAsync(p => p.Guid == guid);
        if (post == null)
            throw new Exception("post not fund");
        //TODO test if works
        var like = _context
            .Posts.Where(p => p.Guid == guid)
            .SelectMany(p => p.Likes)
            .FirstOrDefault(l => l.UserId == user.Id);
        if (like != null)
        {
            _context.Likes.Remove(like);
        }
        else
        {
            var newLike = new Like { UserId = user.Id, PostId = post.Id };
            await _context.Likes.AddAsync(newLike);
        }
        await _context.SaveChangesAsync();
        return post.Likes.Count();
    }

    public async Task<List<Comment>> PostComment(string guid, NewCommentDto commentDto)
    {
        var newComment = _mapper.Map<Comment>(commentDto);

        var post = await _context.Posts.FirstOrDefaultAsync(p => p.Guid == guid);
        if (post == null)
            throw new Exception("post not found");

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Guid == commentDto.UserGuid);
        if (user == null)
            throw new Exception("User not found");

        newComment.User = user;
        newComment.Post = post;

        await _context.Comments.AddAsync(newComment);
        return await _context.Comments.Where(c => c.Post.Guid == guid).ToListAsync();
    }

    public async Task<int> SavePost(string guid, User user)
    {
        //TODO check if works
        var post = await _context.Posts.FirstOrDefaultAsync(p => p.Guid == guid);
        if (post == null)
            throw new Exception("post not found");
        var save = _context
            .Posts.Where(p => p.Guid == guid)
            .SelectMany(p => p.Saveds)
            .FirstOrDefault(s => s.UserId == user.Id);
        if (save != null)
        {
            _context.Saveds.Remove(save);
        }
        else
        {
            var newSave = new Saved { PostId = post.Id, UserId = user.Id };
            await _context.Saveds.AddAsync(newSave);
        }
        await _context.SaveChangesAsync();
        return post.Saveds.Count();
    }
}
