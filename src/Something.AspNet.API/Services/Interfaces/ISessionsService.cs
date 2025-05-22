using Something.AspNet.Database.Models;

namespace Something.AspNet.API.Services.Interfaces;

public interface ISessionsService
{
    public Task<Session> CreateAsync(Guid userId, CancellationToken cancellationToken);

    public Task<bool> IsValidAsync(Guid sessionId, CancellationToken cancellationToken);

    public Task UpdateAsync(Session session, CancellationToken cancellationToken);

    public Task RemoveAsync(Guid sessionId, CancellationToken cancellationToken);

    public Task RefreshAsync(Session session, CancellationToken cancellationToken);
}