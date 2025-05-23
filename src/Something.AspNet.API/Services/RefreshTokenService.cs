using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Something.AspNet.API.Models;
using Something.AspNet.API.Models.Options;
using Something.AspNet.API.Services.Interfaces;
using Something.AspNet.Database.Models;
using System.Text;

namespace Something.AspNet.API.Services;

internal class RefreshTokenService(
    IOptions<JwtOptions> jwtOptions)
    : JwtService, IRefreshTokenService
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
            Encoding.UTF8.GetBytes(jwtOptions.Value.RefreshTokenKey)),
        ClockSkew = TimeSpan.Zero
    };

    public string Create(Session session)
    {
        return Create(
            session.UserId.ToString(),
            session.Id.ToString(),
            session.JwtId.ToString(),
            session.TokensUpdatedAt.UtcDateTime,
            session.RefreshTokenExpiresAt.UtcDateTime,
            _validationParameters);
    }

    public SessionPrincipal? Validate(string token)
    {
        return Validate(token, _validationParameters);
    }
}