using Microsoft.AspNetCore.Authorization;
using Something.AspNet.Auth.API.AuthenticationHandlers;

namespace Something.AspNet.Auth.API.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
internal class JwtAuthorizeAttribute : AuthorizeAttribute
{
    public JwtAuthorizeAttribute()
    {
        AuthenticationSchemes = JwtAuthenticationHandler.SCHEME_NAME;
    }
}