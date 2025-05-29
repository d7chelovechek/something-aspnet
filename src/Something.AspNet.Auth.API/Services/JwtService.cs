using Microsoft.IdentityModel.Tokens;
using Something.AspNet.Auth.API.Constants;
using Something.AspNet.Auth.API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Something.AspNet.Auth.API.Services;

internal abstract class JwtService
{
    protected static string Create(
        string userId,
        string sessionId,
        string jwtId,
        DateTime notBefore,
        DateTime expires,
        TokenValidationParameters validationParameters)
    {
        var claims = new HashSet<Claim>(5)
        {
            new(JwtClaimTypes.Audience, validationParameters.ValidAudience),
            new(JwtClaimTypes.Issuer, validationParameters.ValidIssuer),
            new(JwtClaimTypes.SessionId, sessionId),
            new(JwtClaimTypes.JwtId, jwtId),
            new(JwtClaimTypes.UserId, userId)
        };

        var credentials = new SigningCredentials(
            validationParameters.IssuerSigningKey, 
            SecurityAlgorithms.HmacSha256);

        var securityToken = new JwtSecurityToken(
           claims: claims,
           notBefore: notBefore,
           expires: expires,
           signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }

    protected static SessionPrincipal? Validate(
        string token, 
        TokenValidationParameters validationParameters)
    {
        try
        {
            var principal = new JwtSecurityTokenHandler().ValidateToken(
                token, 
                validationParameters, 
                out _);

            return new SessionPrincipal(principal);
        }
        catch
        {
            return null;
        }
    }
}