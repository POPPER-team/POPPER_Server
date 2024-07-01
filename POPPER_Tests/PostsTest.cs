using Moq;
using POPPER_Server.Models;
using POPPER_Server.Services;

namespace POPPER_Tests;

public class PostsTest
{
    private readonly Mock<IPostService> _postService;

    public PostsTest()
    {
        _postService = new Mock<IPostService>();
    }

    [Fact]
    public async Task LikePostTest()
    {
        var user = new User { Id = 1, Username = "gordan" };
        var post = new Post { Id = 1 };
        var expected = true;

        //        _postService.Setup(x => x.LikePostAsync(It.IsAny<User>(), It.IsAny<Post>()))
        //          .ReturnsAsync(expected);

        //    var result = await _postService.Object.LikePostAsync(user, post);

        //  Assert.Equal(expected, result);
    }

    [Fact]
    public async Task UnlikePostTest()
    {
        var user = new User { Id = 1, Username = "gordan" };
        var post = new Post { Id = 1 };
        var expected = true;

        // _postService.Setup(x => x.UnlikePostAsync(It.IsAny<User>(), It.IsAny<Post>()))
        //   .ReturnsAsync(expected);

        //var result = await _postService.Object.UnlikePostAsync(user, post);

        //  Assert.Equal(expected, result);
    }

    [Fact]
    public async Task SharePostTest()
    {
        var user = new User { Id = 1, Username = "gordan" };
        var post = new Post { Id = 1 };
        var expected = true;

        //_postService.Setup(x => x.SharePostAsync(It.IsAny<User>(), It.IsAny<Post>()))
        //  .ReturnsAsync(expected);

        //var result = await _postService.Object.SharePostAsync(user, post);

        // Assert.Equal(expected, result);
    }
}

