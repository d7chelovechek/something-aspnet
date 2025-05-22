using Microsoft.IdentityModel.Tokens;
using Something.AspNet.API.Constants;
using Something.AspNet.API.Responses;
using Something.AspNet.Database.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Something.AspNet.API.Services.Auth;

internal abstract class JwtService
{
    protected static string CreateToken(
        Session session,
        TokenValidationParameters validationParameters, 
        int expiresAfterMinutes)
    {
        var claims = new HashSet<Claim>(4)
        {
            new(JwtClaimTypes.Audience, validationParameters.ValidAudience),
            new(JwtClaimTypes.Issuer, validationParameters.ValidIssuer),
            new(JwtClaimTypes.SessionId, session.Id.ToString()),
            new(JwtClaimTypes.JwtId, session.JwtId.ToString()),
        };

        var credentials = new SigningCredentials(
            validationParameters.IssuerSigningKey, 
            SecurityAlgorithms.HmacSha256);

        var securityToken = new JwtSecurityToken(
           claims: claims,
           notBefore: session.CreatedAt.UtcDateTime,
           expires: session.CreatedAt.UtcDateTime.AddMinutes(expiresAfterMinutes),
           signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }
}