using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Something.AspNet.API.Constants;
using Something.AspNet.API.Requests;
using Something.AspNet.API.Responses;
using Something.AspNet.API.Services.Auth.Interfaces;

namespace Something.AspNet.API.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        await _authService.RegisterAsync(request, cancellationToken);

        return Created();
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _authService.LoginAsync(request, cancellationToken);

        AppendTokenCookie(CookieNames.ACCESS_TOKEN, response.AccessToken);
        AppendTokenCookie(CookieNames.REFRESH_TOKEN, response.RefreshToken);

        return Ok();
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> LogoutAsync(
        CancellationToken cancellationToken)
    {
        var accessToken = Request.Cookies[CookieNames.ACCESS_TOKEN];

        Response.Cookies.Delete(CookieNames.ACCESS_TOKEN);
        Response.Cookies.Delete(CookieNames.REFRESH_TOKEN);

        await _authService.LogoutAsync(accessToken!, cancellationToken);

        return Ok();
    }

    private void AppendTokenCookie(string tokenName, TokenResponse response)
    {
        Response.Cookies.Append(
            tokenName,
            response.Token,
            new CookieOptions()
            {
                HttpOnly = true,
                IsEssential = true,
                Secure = true,
                Expires = response.ExpiredAt,
                SameSite = SameSiteMode.Strict
            });
    }
}