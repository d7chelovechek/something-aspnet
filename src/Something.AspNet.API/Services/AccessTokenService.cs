using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Something.AspNet.API.Options;
using Something.AspNet.API.Services.Interfaces;
using Something.AspNet.Database.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Something.AspNet.API.Services;

internal class AccessTokenService(IOptions<JwtOptions> jwtOptions)
    : JwtService, IAccessTokenService
{
    private readonly TokenValidationParameters _validationParameters = new()
    {
        ValidateLifetime = true,
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateIssuerSigningKey = true,
        ValidAudience = jwtOptions.Value.Audience,
        ValidIssuer = jwtOptions.Value.Issuer,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtOptions.Value.AccessTokenKey)),
        ClockSkew = TimeSpan.Zero
    };

    public string CreateToken(Session session)
    {
        return CreateToken(
            session.Id.ToString(),
            session.JwtId.ToString(),
            session.TokensUpdatedAt.UtcDateTime,
            session.AccessTokenExpiresAt.UtcDateTime,
            _validationParameters);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            
            return handler.ValidateToken(token, _validationParameters, out _);
        }
        catch
        {
            return null;
        }
    }
}