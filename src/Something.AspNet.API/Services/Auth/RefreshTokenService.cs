using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Something.AspNet.API.Options;
using Something.AspNet.API.Responses;
using Something.AspNet.API.Services.Auth.Interfaces;
using Something.AspNet.Database.Models;
using System.Text;

namespace Something.AspNet.API.Services.Auth;

internal class RefreshTokenService(
    IOptions<JwtOptions> jwtOptions)
    : JwtService, IRefreshTokenService
{
    private readonly TokenValidationParameters _validationParameters = new()
    {
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateIssuerSigningKey = true,
        ValidAudience = jwtOptions.Value.Audience,
        ValidIssuer = jwtOptions.Value.Issuer,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtOptions.Value.RefreshTokenKey))
    };
    private readonly int _expiresInMinutes = jwtOptions.Value.RefreshTokenLifetimeInMinutes;

    public string CreateToken(Session session)
    {
        return CreateToken(
            session,
            _validationParameters,
            _expiresInMinutes);
    }
}