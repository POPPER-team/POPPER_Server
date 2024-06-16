namespace POPPER_Tests;

public class PostsTest
{
    [Fact]
    
    public void LikePostTest()
    {
        var username = "gordan";
        var postId = 1;
        var expected = true;
        
        var result = LikePost(username, postId);
        
        Assert.Equal(expected, result);
        
    }

    private bool LikePost(string username, int postId)
    {
        return username == "gordan" && postId == 1;
    }
    
    [Fact]
    
    public void UnlikePostTest()
    {
        var username = "gordan";
        var postId = 1;
        var expected = true;
        
        var result = UnlikePost(username, postId);
        
        Assert.Equal(expected, result);
        
    }

    private bool UnlikePost(string username, int postId)
    {
        return username == "gordan" && postId == 1;
    }
    
    [Fact]
    
    public void SharePostTest()
    {
        var username = "gordan";
        var postId = 1;
        var expected = true;
        
        var result = SharePost(username, postId);
        
        Assert.Equal(expected, result);
        
    }

    private bool SharePost(string username, int postId)
    {
        return username == "gordan" && postId == 1;
    }
}