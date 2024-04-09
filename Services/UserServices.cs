using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POPPER_Server.Dtos;
using POPPER_Server.Models;

namespace POPPER_Server.Services;

public interface IUserServices
{
    public Task<User> GetUserAsync(string userGuid);

    public Task<string> LoginUserAsync(string username, string password);

    public Task<string> RefreshTokenAsync(string refreshToken);

    public Task<User> RegisterUserAsync(User user);

    public Task<IEnumerable<User>> SearchUserAsync(string searchString);
}

/// <summary>
/// Used for user related operations such as login, register, search and get user
/// </summary>
public class UserServices : IUserServices
{
    private readonly PopperdbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    public UserServices(PopperdbContext context, IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

/// <summary>
/// Retrieves a user based on the user guid.
/// </summary>
/// <param name="userGuid"></param>
/// <returns>User</returns>
    public async Task<User> GetUserAsync(string userGuid)
    {
        User user = await _context.Users.FirstOrDefaultAsync(u => u.Guid == userGuid);
        if (user == null) throw new Exception("User not found");
        return user;
    }

    /// <summary>
    /// Logs in a user based on the username and password
    /// </summary>
    /// <param name="username">User username or email</param>
    /// <param name="password">user password in plain text it will be hashed later</param>
    /// <returns>JWT token</returns>
    async Task<string> IUserServices.LoginUserAsync(string username, string password)
    {
        User user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

        //TODO need to return jwt tokens 
        if (user == null) return "Nop";
        var result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
        if (result == PasswordVerificationResult.Failed) return "Nop";
        return "Success";
    }

    public Task<string> RefreshTokenAsync(string refreshToken)
    {
        //TODO add refresh token logic that returns a new jwt token
        throw new NotImplementedException();
    }

    /// <summary>
    /// Creates a new user in the database. New user will not be logged in
    /// </summary>
    /// <param name="user">User to be created</param>
    /// <returns>Newly created user </returns>
    public async Task<User> RegisterUserAsync(User user)
    {
        user.Password = _passwordHasher.HashPassword(user, user.Password);
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }

    /// <summary>
    /// Function that searches for users based on a search string
    /// The search string is compared to the username, first name and last name of the user
    /// </summary>
    /// <param name="searchString">string to search for in username, first name or last name </param>
    /// <returns>IEnumerable of users</returns>
    public async Task<IEnumerable<User>> SearchUserAsync(string searchString)
    {
        string s = searchString.Trim().ToLower();
        List<User> users = await _context.Users.Where(user =>
            user.Username.ToLower().Contains(s)
            || user.FirstName.Contains(s)
            || user.LastName.Contains(s)
        ).ToListAsync();

        return users;
    }
}