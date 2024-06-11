using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using POPPER_Server.Models;
using POPPER_Server.Services;

namespace POPPER_Server.Helpers;

public static class TokenHelper
{
    private static IServiceProvider _serviceProvider;

    public static void ProvideService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public static string GenerateRefreshToken()
    {
        byte[] randomNumber = new byte[32];
        using RandomNumberGenerator? rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public static async Task<string> GenerateJwtToken(User user)
    {
        IConfiguration _configuration = _serviceProvider.GetRequiredService<IConfiguration>();
        ISessionService _session = _serviceProvider.GetRequiredService<ISessionService>();

        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        Byte[] key = Encoding.ASCII.GetBytes(_configuration["JWT:SecureKey"]);
        string sessionGuid = await _session.GetOrCreateSession(user);
        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("UserGuid", user.Guid),
                new Claim("SessionGuid", sessionGuid)
            }),
            Expires = DateTime.UtcNow.AddDays(Convert.ToInt32(_configuration["JWT:ExpiryInDays"])),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        SecurityToken? token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public static async Task<string> RefreshJwtTokenAsync(string refreshToken)
    {
        PopperdbContext _context = _serviceProvider.GetRequiredService<PopperdbContext>();

        ClaimsPrincipal? principal = GetPrincipalFromExpiredToken(refreshToken);
        string userId = principal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        User user = await _context.Users.FirstOrDefaultAsync(u => Equals(u.Id, userId));
        if (user == null)
            throw new Exception("User not found");

        return await GenerateJwtToken(user);
    }

    public static ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        IConfiguration _configuration = _serviceProvider.GetRequiredService<IConfiguration>();

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
}