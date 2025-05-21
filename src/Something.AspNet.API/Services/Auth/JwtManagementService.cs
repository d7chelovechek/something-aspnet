using Microsoft.IdentityModel.Tokens;
using Something.AspNet.API.Constants;
using Something.AspNet.API.Responses;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Something.AspNet.API.Services.Auth;

internal abstract class JwtManagementService
{
    protected static TokenResponse CreateToken(
        string issuer,
        string audience,
        Guid userId,
        Guid sessionId,
        string securityKey, 
        int expiresAfterMinutes,
        DateTime now)
    {
        var claims = new HashSet<Claim>(4)
        {
            new(JwtClaimTypes.Audience, audience),
            new(JwtClaimTypes.Issuer, issuer),
            new(JwtClaimTypes.SessionId, sessionId.ToString()),
            new(JwtClaimTypes.Subject, userId.ToString())
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey)), 
            SecurityAlgorithms.HmacSha256);

        var expiredAt = now.AddMinutes(expiresAfterMinutes);

        var securityToken = new JwtSecurityToken(
           claims: claims,
           notBefore: now,
           expires: now.AddMinutes(expiresAfterMinutes),
           signingCredentials: credentials);

        return new TokenResponse(
            new JwtSecurityTokenHandler().WriteToken(securityToken),
            expiredAt);
    }
}