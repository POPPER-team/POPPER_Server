using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using POPPER_Server.Dtos;
using POPPER_Server.Models;

namespace POPPER_Server.Services;

public interface IUserServices
{
    public Task<User> GetUserAsync(string userGuid);

    public Task<string> LoginUserAsync(string username, string password);

    public Task<TokensDto> RefreshTokenAsync(string refreshToken);

    public Task<User> RegisterUserAsync(NewUserDto user);

    public Task<IEnumerable<User>> SearchUserAsync(string searchString);
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
    async Task<string> IUserServices.LoginUserAsync(string username, string password)
    {
        User user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

        if (user == null) return "User not found";
        PasswordVerificationResult result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
        if (result == PasswordVerificationResult.Failed) return "Invalid password";
        return GenerateJwtToken(user);
    }

    private string GenerateJwtToken(User user)
    {
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        Byte[] key = Encoding.ASCII.GetBytes(_configuration["JWT:SecureKey"]);
        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            }),
            Expires = DateTime.UtcNow.AddDays(Convert.ToInt32(_configuration["JWT:ExpiryInDays"])),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        SecurityToken? token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task<TokensDto> RefreshTokenAsync(string refreshToken)
    {
        ClaimsPrincipal? principal = GetPrincipalFromExpiredToken(refreshToken);
        string userId = principal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        User user = await _context.Users.FirstOrDefaultAsync(u => Equals(u.Id, userId));
        if (user == null)
        {
            throw new Exception("User not found");
        }

        string jwtToken = GenerateJwtToken(user);
        string newRefreshToken = GenerateRefreshToken();

        return new TokensDto()
        {
            JwtToken = jwtToken,
            RefreshToken = newRefreshToken
        };
    }

    private string GenerateRefreshToken()
    {
        byte[] randomNumber = new byte[32];
        using RandomNumberGenerator? rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:SecureKey"]))
        };

        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        ClaimsPrincipal principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        JwtSecurityToken? jwtSecurityToken = securityToken as JwtSecurityToken;
        if (jwtSecurityToken == null
            || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal;
    }

    /// <summary>
    /// Creates a new user in the database. New user will not be logged in
    /// </summary>
    /// <param name="user">User to be created</param>
    /// <returns>Newly created user </returns>
    public async Task<User> RegisterUserAsync(NewUserDto user)
    {
        User newUser = _mapper.Map<User>(user);
        user.Password = _passwordHasher.HashPassword(newUser, user.Password);
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