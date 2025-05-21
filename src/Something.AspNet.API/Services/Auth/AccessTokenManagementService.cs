using Microsoft.Extensions.Options;
using Something.AspNet.API.Options;
using Something.AspNet.API.Responses;
using Something.AspNet.API.Services.Auth.Interfaces;

namespace Something.AspNet.API.Services.Auth;

internal class AccessTokenManagementService(
    IOptions<JwtOptions> jwtOptions)
    : JwtManagementService, IAccessTokenManagementService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public TokenResponse CreateToken(Guid userId, Guid sessionId, DateTime now)
    {
        return CreateToken(
            _jwtOptions.Issuer,
            _jwtOptions.Audience,
            userId,
            sessionId,
            _jwtOptions.AccessTokenKey,
            _jwtOptions.AccessTokenLifetime,
            now);
    }
}