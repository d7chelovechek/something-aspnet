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
    public async Task<IActionResult> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        await _authService.RegisterAsync(request, cancellationToken);

        return Created();
    }

    [HttpPost("login")]
    public async Task<LoginResponse> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        return await _authService.LoginAsync(request, cancellationToken);
    }

    [HttpPost("logout")]
    [JwtAuthorize]
    public async Task<IActionResult> LogoutAsync(
        CancellationToken cancellationToken)
    {
        return BadRequest();
    }
}