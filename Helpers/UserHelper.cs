using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using POPPER_Server.Models;
using POPPER_Server.Services;

namespace POPPER_Server.Helpers;

public static class UserHelper
{
    private static PopperdbContext _context;
    private static ISessionService _session;

    public static void ProvideService(IServiceProvider services)
    {
        _context = services.GetRequiredService<PopperdbContext>();
        _session = services.GetRequiredService<ISessionService>();
    }

    public static async Task<User> GetUserAsync(this HttpRequest request)
    {
        string userGuid = RetrieveFromRequest("UserGuid", request);
        if (userGuid == null) throw new Exception("User not found");
        User user = await _context.Users.FirstOrDefaultAsync(u => u.Guid == userGuid);
        if (user == null) throw new Exception("User not found");
        return user;
    }

    public static async Task<Session> GetSessionAsync(this HttpRequest request)
    {
        string sessionGuid = RetrieveFromRequest("SessionGuid", request);
        return await _session.GetSession(sessionGuid); 
    }

    public static ClaimsPrincipal GetPrincipalFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);

        var claims = new ClaimsIdentity(jwtToken.Claims);
        var principal = new ClaimsPrincipal(claims);

        return principal;
    }

    private static string RetrieveFromRequest(string identifier, HttpRequest request)
    {
        if (!request.Headers.ContainsKey("Authorization"))
            return null;
        string authorizationHeader = request.Headers["Authorization"];
        if (!authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return null;
        string token = authorizationHeader.Substring("Bearer ".Length).Trim();
        ClaimsPrincipal claims = GetPrincipalFromToken(token);

        return claims.Claims.First(c => c.Type == identifier).Value;
    }
}