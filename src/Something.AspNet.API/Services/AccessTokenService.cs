using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Something.AspNet.API.Database.Models;
using Something.AspNet.API.Models;
using Something.AspNet.API.Models.Options;
using Something.AspNet.API.Services.Interfaces;
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

    public string Create(Session session)
    {
        return Create(
            session.UserId.ToString(),
            session.Id.ToString(),
            session.JwtId.ToString(),
            session.TokensUpdatedAt.UtcDateTime,
            session.AccessTokenExpiresAt.UtcDateTime,
            _validationParameters);
    }

    public SessionPrincipal? Validate(string token)
    {
        return Validate(token, _validationParameters);
    }
}