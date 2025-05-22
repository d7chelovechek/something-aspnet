using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Something.AspNet.API.Constants;
using Something.AspNet.API.Extensions;
using Something.AspNet.API.Services.Auth.Interfaces;
using Something.AspNet.API.Services.Cache.Interfaces;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Something.AspNet.API.AuthenticationHandlers
{
    internal class JwtAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> authSchemeOptions,
        ILoggerFactory loggerFactory,
        UrlEncoder urlEncoder,
        IAccessTokenService accessTokenService,
        ISessionsService sessionsService)
        : AuthenticationHandler<AuthenticationSchemeOptions>(
            authSchemeOptions,
            loggerFactory,
            urlEncoder)
    {
        public const string SCHEME_NAME = "JwtAuthenticationScheme";

        private const string BEARER = "Bearer ";

        private readonly IAccessTokenService _accessTokenService = accessTokenService;
        private readonly ISessionsService _sessionsService = sessionsService;

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var authorization = Request.Headers.Authorization.ToString();

            if (string.IsNullOrWhiteSpace(authorization))
            {
                return AuthenticateResult.Fail("Authorization header not found");
            }
                
            if (!authorization.StartsWith(BEARER))
            {
                return AuthenticateResult.Fail("Bearer prefix not found");
            }

            string accessToken = authorization[BEARER.Length..];

            if (_accessTokenService.ValidateToken(accessToken) is not ClaimsPrincipal principal ||
                principal.GetSessionId() is not Guid sessionId ||
                principal.GetExpiresAt() is not DateTimeOffset expiresAt)
            {
                return AuthenticateResult.Fail("AccessToken is invalid");
            }

            bool isValid = await _sessionsService.ValidateAsync(
                sessionId, 
                expiresAt,
                CancellationToken.None);

            if (!isValid)
            {
                return AuthenticateResult.Fail("AccessToken is invalid");
            }

            return AuthenticateResult.Success(new AuthenticationTicket(principal, SCHEME_NAME));
        }
    }
}