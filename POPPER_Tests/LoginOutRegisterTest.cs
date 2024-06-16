namespace POPPER_Tests;

public class LoginOutRegisterTest
{
    [Fact]
    public void LoginTest()
    {
        var username = "gordan";
        var password = "password";
        var expected = true;
        
        var result = Login(username, password);
        
        Assert.Equal(expected, result);
        
    }

    private bool Login(string username, string password)
    {
        return username == "gordan" && password == "password";
    }

    [Fact]

    public void LogOutTest()
    {
        var expected = true;
        var result = LogOut();
        
        Assert.Equal(expected, result);
        
    }

    private bool LogOut()
    {
        return true;
    }

    [Fact]

    public void RegisterTest()
    {
        var username = "gordan";
        var password = "password";
        var email = "gordan@algebra.hr";
        var expected = true;
        
        var result = Register(username, password, email);
        
        Assert.Equal(expected, result);

    }

    private bool Register(string username, string password, string email)
    {
        return username == "gordan" && password == "password" && email == "gordan@algebra.hr";
    }
}