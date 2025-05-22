using Something.AspNet.API.Constants;
using System.Security.Claims;

namespace Something.AspNet.API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid? GetSessionId(this ClaimsPrincipal principal)
        {
            if (principal.Claims.FirstOrDefault(c => c.Type is JwtClaimTypes.SessionId)?.Value 
                    is not string strSessionId)
            {
                return null;
            }

            if (Guid.TryParse(strSessionId, out Guid sessionId))
            {
                return sessionId;
            }

            return null;
        }

        public static DateTimeOffset? GetExpiresAt(this ClaimsPrincipal principal)
        {
            if (principal.Claims.FirstOrDefault(c => c.Type is JwtClaimTypes.ExpiresAt)?.Value
                    is not string strExpiresAt)
            {
                return null;
            }

            if (long.TryParse(strExpiresAt, out long expiresAtTimestamp))
            {
                return DateTimeOffset.FromUnixTimeSeconds(expiresAtTimestamp);
            }

            return null;
        }
    }
}