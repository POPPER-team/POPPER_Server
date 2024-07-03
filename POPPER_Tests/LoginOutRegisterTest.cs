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
        var mockUserServices = new Mock<IUserServices>();
        var expectedTokens = new TokensDto();
        var username = "Gordan";
        var password = "Pa$$w0rd";

        mockUserServices
            .Setup(service => service.LoginUserAsync(username, password))
            .ReturnsAsync(expectedTokens);


        var result = await _userServices.LoginUserAsync(username, password);

        Assert.Equal(expectedTokens, result);
    }

    [Fact]
    public async Task RegisterTest()
    {
        
        var mockUserServices = new Mock<IUserServices>();
        var expectedUser = new User();
        var newUser = new NewUserDto()
        {
            Username = "Gordan",
            FirstName = "Gordan",
            LastName = "Ramzi",
            Password = "Pa$$w0rd",
            DateOfBirth = "1966/11/8",
            Email = "gramsi@kuhinja.hr"
        };

        mockUserServices
            .Setup(service => service.RegisterUserAsync(newUser))
            .ReturnsAsync(expectedUser);
        
        var result = await _userServices.RegisterUserAsync(newUser);
        
        Assert.Equal(expectedUser, result);

     }

}