using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using POPPER_Server.Models;

namespace POPPER_Server.Helpers;

public static class TokenHelper
{
    private static PopperdbContext _context;
    private static IConfiguration _configuration;

    public static void ProvideService(IServiceProvider serviceProvider)
    {
        _context = serviceProvider.GetRequiredService<PopperdbContext>();
        _configuration = serviceProvider.GetRequiredService<IConfiguration>();
    }

    public static string GenerateRefreshToken()
    {
        byte[] randomNumber = new byte[32];
        using RandomNumberGenerator? rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public static string GenerateJwtToken(User user)
    {
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        Byte[] key = Encoding.ASCII.GetBytes(_configuration["JWT:SecureKey"]);
        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("UserGuid", user.Guid),
                new Claim("SessionGuid", "")
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
        ClaimsPrincipal? principal = GetPrincipalFromExpiredToken(refreshToken);
        string userId = principal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        User user = await _context.Users.FirstOrDefaultAsync(u => Equals(u.Id, userId));
        if (user == null)
            throw new Exception("User not found");

        return GenerateJwtToken(user);
    }

    public static ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
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
}