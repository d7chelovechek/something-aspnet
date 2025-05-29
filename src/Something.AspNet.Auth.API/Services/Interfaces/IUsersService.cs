using Something.AspNet.Auth.API.Requests;

namespace Something.AspNet.Auth.API.Services.Interfaces;

public interface IUsersService
{
    public Task<Guid> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken);

    public Task RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken);
}