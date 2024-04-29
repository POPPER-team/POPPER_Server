using POPPER_Server.Dtos;

namespace POPPER_Server.Services;

public interface IFollowService
{
    public Task FollowUserAsync(string userGuid, string followingGuid);
    public Task UnFollowUserAsync(string userGuid, string followingGuid);
    public Task<List<UserDto>> GetFollowersAsync(string userGuid);
    public Task<List<UserDto>> GetFollowingAsync(string userGuid);
}




