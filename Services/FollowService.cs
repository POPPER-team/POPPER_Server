using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MongoDB.Driver;
using POPPER_Server.Dtos;
using POPPER_Server.Models;

namespace POPPER_Server.Services;

public interface IFollowService
{
    public Task FollowUserAsync(string userGuid, string followingGuid);
    public Task UnFollowUserAsync(string userGuid, string followingGuid);
    public Task<List<User>> GetFollowersAsync(User user);
    public Task<List<User>> GetFollowingAsync(User user);
}

public class FollowService : IFollowService
{
    private readonly PopperdbContext _context;

    public FollowService(PopperdbContext context)
    {
        _context = context;
    }


    public async Task FollowUserAsync(string userGuid, string followingGuid)
    {
       User user = await _context.Users.FirstOrDefaultAsync(u => u.Guid == userGuid);
       User followUser = await _context.Users.FirstOrDefaultAsync(u => u.Guid == followingGuid);

       if (user.Guid == userGuid && followUser.Guid == followingGuid)
       {
              throw new Exception("You can't follow yourself or that user again");
       }
       
        Following follow = new Following()
        {
            UserId = user.Id,
            FollowingId = followUser.Id
        };

        _context.Followings.Add(follow);
        await _context.SaveChangesAsync();
    }

    public async Task UnFollowUserAsync(string userGuid, string followingGuid)
    {
        User user = await _context.Users.FirstOrDefaultAsync(u => u.Guid == userGuid);
        User followUser = await _context.Users.FirstOrDefaultAsync(u => u.Guid == followingGuid);
        
        
        Following follow = await _context.Followings.FirstOrDefaultAsync(f =>
            f.UserId == user.Id && f.FollowingId == followUser.Id);
        
        
        
        if (follow != null)
        {
            _context.Followings.Remove(follow);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<User>> GetFollowersAsync(User user)
    {
        return await _context.Followings
            .Where(f => f.FollowingId == user.Id)
            .Include(u => u.User)
            .Select(f => f.User)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<User>> GetFollowingAsync(User user)
    {
        
        return await _context.Followings
            .Where(f => f.UserId == user.Id)
            .Include(u => u.User)
            .Select(f => f.FollowingNavigation)
            .AsNoTracking()
            .ToListAsync();
    }
}