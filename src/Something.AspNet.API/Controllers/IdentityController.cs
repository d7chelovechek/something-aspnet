using Microsoft.AspNetCore.Mvc;
using Something.AspNet.API.Attributes;
using Something.AspNet.API.Requests;
using Something.AspNet.API.Services.Interfaces;

namespace Something.AspNet.API.Controllers;

[ApiController]
[Route("identity")]
public class IdentityController(
    IUsersService userService,
    ISessionsService sessionsService) : ApiControllerBase
{
    private readonly IUsersService _userService = userService;
    private readonly ISessionsService _sessionsService = sessionsService;

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUserAsync(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        await _userService.RegisterAsync(request, cancellationToken);

        return Created();
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUserAsync(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var userId = await _userService.LoginAsync(request, cancellationToken);
        var response = await _sessionsService.CreateAsync(userId, cancellationToken);

        return Ok(response);
    }

    [HttpPost("logout")]
    [JwtAuthorize]
    public async Task<IActionResult> LogoutUserAsync(
        CancellationToken cancellationToken)
    {
        await _sessionsService.RemoveAsync(Session.SessionId, cancellationToken);

        return Ok();
    }

    [HttpPost("sessions")]
    public async Task<IActionResult> RefreshSessionAsync(
        RefreshSessionRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _sessionsService.RefreshAsync(request, cancellationToken);

        return Ok(response);
    }

    [HttpGet("sessions")]
    [JwtAuthorize]
    public async Task<IActionResult> GetSessionsAsync(CancellationToken cancellationToken)
    {
        var response = await _sessionsService.GetAsync(Session.UserId, cancellationToken);

        return Ok(response);
    }
}