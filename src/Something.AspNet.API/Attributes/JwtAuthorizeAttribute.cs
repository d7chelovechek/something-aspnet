using Microsoft.AspNetCore.Authorization;
using Something.AspNet.API.AuthenticationHandlers;

namespace Something.AspNet.API.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
internal class JwtAuthorizeAttribute : AuthorizeAttribute
{
    public JwtAuthorizeAttribute()
    {
        AuthenticationSchemes = JwtAuthenticationHandler.SCHEME_NAME;
    }
}