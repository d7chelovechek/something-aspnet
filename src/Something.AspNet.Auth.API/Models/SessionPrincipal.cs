using Something.AspNet.Auth.API.Constants;
using Something.AspNet.Auth.API.Exceptions;
using System.Security.Claims;

namespace Something.AspNet.Auth.API.Models;

public class SessionPrincipal(ClaimsPrincipal principal) : ClaimsPrincipal(principal)
{
    public Guid SessionId { get; } = GetSpecifiedId(principal, JwtClaimTypes.SessionId);

    public Guid UserId { get; } = GetSpecifiedId(principal, ClaimTypes.NameIdentifier);

    public Guid JwtId { get; } = GetSpecifiedId(principal, JwtClaimTypes.JwtId);

    private static Guid GetSpecifiedId(ClaimsPrincipal principal, string claimType)
    {
        if (principal.Claims.FirstOrDefault(c => c.Type.Equals(claimType))?.Value
                is string strId &&
            Guid.TryParse(strId, out Guid id))
        {
            return id;
        }

        throw new AuthenticatedSessionInvalidException();
    }
}