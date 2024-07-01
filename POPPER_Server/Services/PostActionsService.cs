using POPPER_Server.Models;

public interface IPostActions
{
    public Task<int> LikePost(string guid);
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

    public Task<int> LikePost(string guid)
    {
        throw new NotImplementedException();
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
