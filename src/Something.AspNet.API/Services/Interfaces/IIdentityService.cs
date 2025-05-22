using Something.AspNet.API.Requests;
using Something.AspNet.API.Responses;

namespace Something.AspNet.API.Services.Interfaces;

public interface IIdentityService
{
    public Task<LoginResponse> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken);

    public Task LogoutAsync(
        Guid sessionId,
        CancellationToken cancellationToken);

    public Task<RefreshResponse> RefreshAsync(
        RefreshRequest request,
        CancellationToken cancellationToken);

    public Task RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken);
}