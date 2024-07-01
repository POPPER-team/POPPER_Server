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

    public PostActions(PopperdbContext context)
    {
        _context = context;
    }

    public Task<string> GetShareLink(string guid)
    {
        throw new NotImplementedException();
    }

    public async Task<int> LikePost(string guid, User user)
    {
        var like = await _context.Likes.FirstOrDefaultAsync(l => l.UserId == user.Id);
        var post = await _context.Posts.FirstOrDefaultAsync(p => p.Guid == guid);
        if (post == null)
            throw new Exception("post not fund");
        if (like != null)
        {
            _context.Remove(like);
        }
        else
        {
            var newLike = new Like { User = user, Post = post };
            await _context.Likes.AddAsync(newLike);
        }
        await _context.SaveChangesAsync();
        return post.Likes.Count();
    }

    public Task<List<Comment>> PostComment(string guid, NewCommentDto commentDto)
    {
        throw new NotImplementedException();
    }

    public Task<int> SavePost(string guid, User user)
    {
        throw new NotImplementedException();
    }
}
