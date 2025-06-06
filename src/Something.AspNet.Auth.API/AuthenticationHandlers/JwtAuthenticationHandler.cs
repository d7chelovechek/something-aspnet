﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Something.AspNet.Auth.API.Models;
using Something.AspNet.Auth.API.Services.Interfaces;
using System.Text.Encodings.Web;

namespace Something.AspNet.Auth.API.AuthenticationHandlers;

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

        if (_accessTokenService.Validate(accessToken) is not SessionPrincipal principal)
        {
            return AuthenticateResult.Fail(ACCESS_TOKEN_IS_INVALID);
        }

        try
        {
            bool isValid = await _sessionsService.IsValidAsync(
                principal.SessionId, 
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