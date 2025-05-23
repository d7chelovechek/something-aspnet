using Something.AspNet.API.Requests;
using Something.AspNet.API.Responses;
using Something.AspNet.Database.Models;

namespace Something.AspNet.API.Services.Interfaces;

public interface ISessionsService
{
    public Task<CreatedSessionResponse> CreateAsync(
        Guid userId, 
        CancellationToken cancellationToken);

    public Task<bool> IsValidAsync(Guid sessionId, CancellationToken cancellationToken);

    public Task<FoundSessionsResponse> GetAsync(Guid userId, CancellationToken cancellationToken);

    public Task UpdateAsync(Session session, CancellationToken cancellationToken);

    public Task RemoveAsync(Guid sessionId, CancellationToken cancellationToken);

    public Task<RefreshedSessionResponse> RefreshAsync(
        RefreshSessionRequest request, 
        CancellationToken cancellationToken);
}