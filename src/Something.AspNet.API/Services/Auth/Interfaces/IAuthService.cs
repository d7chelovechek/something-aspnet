using Something.AspNet.API.Requests;
using Something.AspNet.API.Responses;

namespace Something.AspNet.API.Services.Auth.Interfaces;

public interface IAuthService
{
    public Task<LoginResponse> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken);

    public Task LogoutAsync(
        string accessToken,
        CancellationToken cancellationToken);

    public Task RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken);
}