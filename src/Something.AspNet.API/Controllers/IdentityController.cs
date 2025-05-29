using Microsoft.AspNetCore.Mvc;
using Something.AspNet.API.Attributes;
using Something.AspNet.API.Requests;
using Something.AspNet.API.Responses;
using Something.AspNet.API.Services.Interfaces;
using System.Net;

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
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorsResponse), (int)HttpStatusCode.UnprocessableEntity)]
    [ProducesResponseType(typeof(ErrorsResponse), (int)HttpStatusCode.Conflict)]
    public async Task<IActionResult> RegisterUserAsync(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        await _userService.RegisterAsync(request, cancellationToken);

        return Created();
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(CreatedSessionResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorsResponse), (int)HttpStatusCode.BadRequest)]
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
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> LogoutUserAsync(
        CancellationToken cancellationToken)
    {
        await _sessionsService.RemoveAsync(Session.SessionId, cancellationToken);

        return Ok();
    }

    [HttpPost("sessions")]
    [ProducesResponseType(typeof(RefreshedSessionResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorsResponse), (int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> RefreshSessionAsync(
        RefreshSessionRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _sessionsService.RefreshAsync(request.RefreshToken, cancellationToken);

        return Ok(response);
    }

    [HttpGet("sessions")]
    [JwtAuthorize]
    [ProducesResponseType(typeof(ActiveSessionsResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetActiveSessionsAsync(CancellationToken cancellationToken)
    {
        var response = await _sessionsService.GetActiveAsync(Session.UserId, cancellationToken);

        return Ok(response);
    }

    [HttpDelete("sessions")]
    [JwtAuthorize]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorsResponse), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ErrorsResponse), (int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> RemoveSessionAsync(
        RemoveSessionRequest request,
        CancellationToken cancellationToken)
    {
        await _sessionsService.RemoveWithPrincipalCheckAsync(
            request.SessionId, 
            Session, 
            cancellationToken);

        return Ok();
    }
}