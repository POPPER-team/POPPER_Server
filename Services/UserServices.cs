using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using POPPER_Server.Dtos;
using POPPER_Server.Helpers;
using POPPER_Server.Models;

namespace POPPER_Server.Services;

public interface IUserServices
{
    public Task<User> GetUserAsync(string userGuid);
    public Task<TokensDto> LoginUserAsync(string username, string password);
    public Task<User> RegisterUserAsync(NewUserDto user);
    public Task<IEnumerable<User>> SearchUserAsync(string searchString);
    public Task<string> RefreshJwtTokenAsync(string refreshToken);
}

/// <summary>
/// Used for user related operations such as login, register, search and get user
/// </summary>
public class UserServices : IUserServices
{
    private readonly PopperdbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    public UserServices(PopperdbContext context, IPasswordHasher<User> passwordHasher, IConfiguration configuration,
        IMapper mapper)
    {
        _mapper = mapper;
        _context = context;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
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
    async Task<TokensDto> IUserServices.LoginUserAsync(string username, string password)
    {
        User user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

        if (user == null) throw new Exception("Login failed");
        PasswordVerificationResult result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
        if (result == PasswordVerificationResult.Failed) throw new Exception("Login failed");
        return new TokensDto()
        {
            JwtToken = await TokenHelper.GenerateJwtToken(user),
            RefreshToken = TokenHelper.GenerateRefreshToken()
        };
    }

    public async Task<string> RefreshJwtTokenAsync(string refreshToken)
    {
        ClaimsPrincipal? principal = TokenHelper.GetPrincipalFromExpiredToken(refreshToken);
        string userId = principal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        User user = await _context.Users.FirstOrDefaultAsync(u => Equals(u.Id, userId));
        if (user == null)
        {
            throw new Exception("User not found");
        }

        return await TokenHelper.GenerateJwtToken(user);
    }

    /// <summary>
    /// Creates a new user in the database. New user will not be logged in
    /// </summary>
    /// <param name="user">User to be created</param>
    /// <returns>Newly created user </returns>
    public async Task<User> RegisterUserAsync(NewUserDto user)
    {
        User newUser = _mapper.Map<User>(user);
        newUser.Password = _passwordHasher.HashPassword(newUser, user.Password);
        await _context.Users.AddAsync(newUser);
        await _context.SaveChangesAsync();
        return newUser;
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