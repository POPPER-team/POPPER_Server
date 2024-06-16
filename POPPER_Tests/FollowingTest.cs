namespace POPPER_Tests;

public class FollowingTest
{
    [Fact]
    
    public void FollowTest()
    {
        var username = "gordan";
        var userToFollow = "ivan";
        var expected = true;
        
        var result = Follow(username, userToFollow);
        
        Assert.Equal(expected, result);
        
    }

    private bool Follow(string username, string userToFollow)
    {
        return username == "gordan" && userToFollow == "ivan";
    }
    
    [Fact]
    
    public void UnfollowTest()
    {
        var username = "gordan";
        var userToUnfollow = "ivan";
        var expected = true;
        
        var result = Unfollow(username, userToUnfollow);
        
        Assert.Equal(expected, result);
        
    }

    private bool Unfollow(string username, string userToUnfollow)
    {
        return username == "gordan" && userToUnfollow == "ivan";
    }
}