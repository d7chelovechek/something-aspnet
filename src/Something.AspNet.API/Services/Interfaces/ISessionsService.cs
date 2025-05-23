using Something.AspNet.API.Models;
using Something.AspNet.API.Responses;

namespace Something.AspNet.API.Services.Interfaces;

public interface ISessionsService
{
    public Task<CreatedSessionResponse> CreateAsync(
        Guid userId, 
        CancellationToken cancellationToken);

    public Task<bool> IsValidAsync(Guid sessionId, CancellationToken cancellationToken);

    public Task<FoundSessionsResponse> GetAsync(Guid userId, CancellationToken cancellationToken);

    public Task RemoveAsync(Guid sessionId, CancellationToken cancellationToken);

    public Task RemoveAsync(
        Guid sessionId, 
        SessionPrincipal principal, 
        CancellationToken cancellationToken);

    public Task<RefreshedSessionResponse> RefreshAsync(
        string refreshToken, 
        CancellationToken cancellationToken);
}