using Moq;
using POPPER_Server.Dtos;
using POPPER_Server.Models;

namespace POPPER_Tests;

using POPPER_Server.Services;

public class LoginOutRegisterTest
{
    private readonly IUserServices _userServices;

    public LoginOutRegisterTest()
    {
        var mockUserServices = new Mock<IUserServices>();
        mockUserServices.Setup(service => service.LoginUserAsync( It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new TokensDto());

        mockUserServices.Setup(service => service.RegisterUserAsync(It.IsAny<NewUserDto>()))
            .ReturnsAsync(new User());

        _userServices = mockUserServices.Object;
    }

    [Fact]
    public async Task LoginTest()
    {
        var username = "gordan";
        var password = "password";
        var expected = new TokensDto();

        var result = await _userServices.LoginUserAsync(username, password);

        Assert.Equal(expected, result);
    }

 //Register test is failing, TODO: fix it
    [Fact]
    public async Task RegisterTest()
    {
        var newUser = new NewUserDto()
        {
            Username = "gordan",
            Password = "password",
            Email = "gordan@algebra.hr"
        };
        var expected = new User();
        var result = await _userServices.RegisterUserAsync(newUser);

        Assert.Equal(expected, result);

     }

}