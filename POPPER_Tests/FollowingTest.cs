using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using POPPER_Server.Models;
namespace POPPER_Tests;
using POPPER_Server.Services;

public class FollowingTest
{
    private readonly IFollowService _followingService;
    private readonly Mock<IFollowService> _mockFollowingService;

    public FollowingTest()
    {
        _mockFollowingService = new Mock<IFollowService>();
        _followingService = _mockFollowingService.Object;
    }

    [Fact]
    public async Task FollowTest()
    {
       
        var user = new User
        {
            Guid = Guid.NewGuid().ToString(), Username = "ramiza", Password = "password",
            FirstName = "ramiza", LastName = "ramiz", Created = DateTime.Now
        };
        var userToFollow = new User
        {
            Guid = Guid.NewGuid().ToString(), Username = "ivan", Password = "password",
            FirstName = "ivan", LastName = "ramiz", Created = DateTime.Now
            
        };
        var userToFollowGuid = userToFollow.Guid;
        var followResult = await _followingService.FollowUserAsync(user, userToFollowGuid);

        Assert.False(followResult);

        var followers = await _followingService.GetFollowersAsync(userToFollow);

        var isFollowing = followers?.Any(follower => follower.Id == user.Id);

        Assert.True(isFollowing ?? true, "The current user should be in the list of followers after following the user.");
    }


    [Fact]
    public  async Task UnfollowTest()
    {

        var user = new User
        {
            Guid = Guid.NewGuid().ToString(), Username = "gordan", Password = "password",
            FirstName = "gordin", LastName = "ramiz", Created = DateTime.Now
        };
        var userToUnFollow = new User
        {
            Guid = Guid.NewGuid().ToString(), Username = "ivan", Password = "password",
            FirstName = "ivan", LastName = "ramiz", Created = DateTime.Now
            
        };
        var userToUnFollowGuid = userToUnFollow.Guid;

        var followersBefore = await _followingService.GetFollowersAsync(userToUnFollow);

        var isFollowingBefore = followersBefore?.Any(follower => follower.Id == user.Id);

        Assert.True(isFollowingBefore ?? true, "The current user should be in the list of followers before unfollowing the user.");

        var result = await _followingService.UnFollowUserAsync(user, userToUnFollowGuid);
        Assert.False(result);

        var followersAfter = await _followingService.GetFollowersAsync(userToUnFollow);

        var isFollowingAfter = followersAfter?.Any(follower => follower.Id == user.Id);

        Assert.False(isFollowingAfter ?? false, "The current user should not be in the list of followers after unfollowing the user.");
    }
}