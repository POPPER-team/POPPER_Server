namespace POPPER_Server.Services;

public interface IUserServices
{
    public Task<User> GetUserAsync(string userGuid);

    public Task<string> LoginUserAsync(string username, string password);

    public Task<string> RefreshTokenAsync(string refreshToken);

    public Task<User> RegisterUserAsync(User user);

    public Task<IEnumerable<User>> SearchUserAsync(string searchString);
}

public class User
{
}

public class UserServices : IUserServices
{
    // private readonly DbContext _context;
    public UserServices()
    {
        
    }

    public Task<User> GetUserAsync(string userGuid)
    {
        throw new NotImplementedException();
    }

    Task<string> IUserServices.LoginUserAsync(string username, string password)
    {
        throw new NotImplementedException();
    }

    public Task<string> RefreshTokenAsync(string refreshToken)
    {
        throw new NotImplementedException();
    }

    public Task<User> RegisterUserAsync(User user)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<User>> SearchUserAsync(string searchString)
    {
        throw new NotImplementedException();
    }
}