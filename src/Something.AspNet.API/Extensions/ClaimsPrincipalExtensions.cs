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

    public static DateTimeOffset GetExpiresAt(this ClaimsPrincipal principal)
    {
        if (principal.Claims.FirstOrDefault(c => c.Type is JwtClaimTypes.ExpiresAt)?.Value
                is string strExpiresAt &&
            long.TryParse(strExpiresAt, out long expiresAtTimestamp))
        {
            return DateTimeOffset.FromUnixTimeSeconds(expiresAtTimestamp);
        }

        throw new TokenInvalidException();
    }
}