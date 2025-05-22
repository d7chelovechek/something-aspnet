using Something.AspNet.API.Constants;
using Something.AspNet.API.Exceptions;
using System.Security.Claims;

namespace Something.AspNet.API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetSessionId(this ClaimsPrincipal principal)
    {
        if (principal.Claims.FirstOrDefault(c => c.Type is JwtClaimTypes.SessionId)?.Value 
                is string strSessionId &&
            Guid.TryParse(strSessionId, out Guid sessionId))
        {
            return sessionId;
        }

        throw new TokenInvalidException();
    }

    public static Guid GetJwtId(this ClaimsPrincipal principal)
    {
        if (principal.Claims.FirstOrDefault(c => c.Type is JwtClaimTypes.JwtId)?.Value
                is string strJwtId &&
            Guid.TryParse(strJwtId, out Guid jwtId))
        {
            return jwtId;
        }

        throw new TokenInvalidException();
    }
}