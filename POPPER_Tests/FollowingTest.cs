using Moq;
using POPPER_Server.Models;

namespace POPPER_Tests;

using POPPER_Server.Services;

public class FollowingTest
{
    private readonly Mock<IFollowService> _followingService;

    public FollowingTest()
    {
        _followingService = new Mock<IFollowService>();
    }

    [Fact]
    public async Task FollowTest()
    {
        var user = new User { Id = 1, Username = "gordan" };
        var userToFollow = new User { Id = 2, Username = "ivan" };
        var userToFollowGuid = "2";
        var expected = true;


        var followResult = await _followingService.Object.FollowUserAsync(user, userToFollowGuid);

        Assert.Equal(expected, followResult);

        var followers = await _followingService.Object.GetFollowersAsync(userToFollow);

        var isFollowing = followers.Any(follower => follower.Id == user.Id);

        Assert.True(isFollowing, "The current user should be in the list of followers after following the user.");
    }


    [Fact]
    public  async Task UnfollowTest()
    {
        var user = new User { Id = 1, Username = "gordan" };
        var userToUnFollow = new User { Id = 2, Username = "ivan" };
        var userToUnFollowGuid = "2";
        var expected = true;

        var followersBefore = await _followingService.Object.GetFollowersAsync(userToUnFollow);

        var isFollowingBefore = followersBefore.Any(follower => follower.Id == user.Id);

        Assert.True(isFollowingBefore, "The current user should be in the list of followers before unfollowing the user.");

        var result = await _followingService.Object.UnFollowUserAsync(user, userToUnFollowGuid);
        Assert.Equal(expected, result);

        var followersAfter = await _followingService.Object.GetFollowersAsync(userToUnFollow);

        var isFollowingAfter = followersAfter.Any(follower => follower.Id == user.Id);

        Assert.False(isFollowingAfter, "The current user should not be in the list of followers after unfollowing the user.");
    }
}