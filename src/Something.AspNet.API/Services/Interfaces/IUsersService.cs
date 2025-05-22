using Something.AspNet.API.Requests;

namespace Something.AspNet.API.Services.Interfaces;

public interface IUsersService
{
    public Task<Guid> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken);

    public Task RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken);
}