using Something.AspNet.Database.Models;

namespace Something.AspNet.API.Services.Auth.Interfaces
{
    public interface ISessionsService
    {
        public Task<Session> CreateAsync(Guid userId, CancellationToken cancellationToken);

        public Task<bool> ValidateAsync(
            Guid sessionId, 
            DateTimeOffset expiresAt,
            CancellationToken cancellationToken);

        public Task UpdateAsync(Session session, CancellationToken cancellationToken);

    }
}