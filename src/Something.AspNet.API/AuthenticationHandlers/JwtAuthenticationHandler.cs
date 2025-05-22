using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Something.AspNet.API.Extensions;
using Something.AspNet.API.Services.Interfaces;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Something.AspNet.API.AuthenticationHandlers;

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

    private const string ACCESS_TOKEN_IS_INVALID = "AccessToken is invalid";
    private const string HEADER_IS_INVALID = "Authorization header is invalid";

    private readonly IAccessTokenService _accessTokenService = accessTokenService;
    private readonly ISessionsService _sessionsService = sessionsService;

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authorization = Request.Headers.Authorization.ToString();

        if (string.IsNullOrWhiteSpace(authorization))
        {
            return AuthenticateResult.Fail(HEADER_IS_INVALID);
        }
        
        if (!authorization.StartsWith(BEARER))
        {
            return AuthenticateResult.Fail(HEADER_IS_INVALID);
        }

        string accessToken = authorization[BEARER.Length..];

        if (_accessTokenService.ValidateToken(accessToken) is not ClaimsPrincipal principal)
        {
            return AuthenticateResult.Fail(ACCESS_TOKEN_IS_INVALID);
        }

        try
        {
            bool isValid = await _sessionsService.IsValidAsync(
                principal.GetSessionId(),
                CancellationToken.None);

            if (!isValid)
            {
                return AuthenticateResult.Fail(ACCESS_TOKEN_IS_INVALID);
            }
        }
        catch
        {
            return AuthenticateResult.Fail(ACCESS_TOKEN_IS_INVALID);
        }

        return AuthenticateResult.Success(new AuthenticationTicket(principal, SCHEME_NAME));
    }
}