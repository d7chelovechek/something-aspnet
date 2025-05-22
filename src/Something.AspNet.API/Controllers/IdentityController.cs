using Microsoft.AspNetCore.Mvc;
using Something.AspNet.API.Extensions;
using Something.AspNet.API.Requests;
using Something.AspNet.API.Services.Interfaces;

namespace Something.AspNet.API.Controllers;

[ApiController]
[Route("identity")]
public class IdentityController(IIdentityService identityService) : ControllerBase
{
    private readonly IIdentityService _identityService = identityService;

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        await _identityService.RegisterAsync(request, cancellationToken);

        return Created();
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _identityService.LoginAsync(request, cancellationToken);

        return Ok(response);
    }

    [HttpPost("logout")]
    [JwtAuthorize]
    public async Task<IActionResult> LogoutAsync(
        CancellationToken cancellationToken)
    {
        await _identityService.LogoutAsync(
            User.GetSessionId(),
            cancellationToken);

        return Ok();
    }

    [HttpPost("sessions")]
    public async Task<IActionResult> RefreshAsync(
        RefreshRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _identityService.RefreshAsync(
            request, 
            cancellationToken);

        return Ok(response);
    }
}