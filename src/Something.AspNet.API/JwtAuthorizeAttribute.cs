using Microsoft.AspNetCore.Authorization;
using Something.AspNet.API.AuthenticationHandlers;

namespace Something.AspNet.API
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class JwtAuthorizeAttribute : AuthorizeAttribute
    {
        public JwtAuthorizeAttribute()
        {
            AuthenticationSchemes = JwtAuthenticationHandler.SCHEME_NAME;
        }
    }
}