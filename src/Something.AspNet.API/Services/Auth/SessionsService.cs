using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Something.AspNet.API.Options;
using Something.AspNet.API.Services.Auth.Interfaces;
using Something.AspNet.API.Services.Cache.Interfaces;
using Something.AspNet.Database;
using Something.AspNet.Database.Models;

namespace Something.AspNet.API.Services.Auth
{
    internal class SessionsService(
        IOptions<JwtOptions> jwtOptions,
        IApplicationDbContext dbContext,
        ISessionsCache sessionsCache,
        TimeProvider timeProvider)
        : ISessionsService
    {
        private readonly JwtOptions _jwtOptions = jwtOptions.Value;
        private readonly IApplicationDbContext _dbContext = dbContext;
        private readonly ISessionsCache _sessionsCache = sessionsCache;
        private readonly TimeProvider _timeProvider = timeProvider;

        public async Task<Session> CreateAsync(Guid userId, CancellationToken cancellationToken)
        {
            var now = _timeProvider.GetUtcNow();

            var session = new Session()
            {
                UserId = userId,
                UpdatableTo = now.AddMinutes(_jwtOptions.RefreshTokenLifetimeInMinutes),
                ExpiresAt = now.AddMinutes(_jwtOptions.SessionLifetimeInMinutes),
                JwtId = Guid.NewGuid(),
                CreatedAt = now,
            };

            await UpdateAsync(session, cancellationToken);

            return session;
        }

        public async Task<bool> ValidateAsync(
            Guid sessionId,
            DateTimeOffset expiresAt,
            CancellationToken cancellationToken)
        {
            if (_sessionsCache.Get(sessionId) is bool isValid)
            {
                return isValid;
            }

            Session? existingSession = 
                await _dbContext.Sessions.SingleOrDefaultAsync(
                    s => s.Id.Equals(sessionId), 
                    cancellationToken);

            return _sessionsCache.Update(
                sessionId, 
                existingSession is not null, 
                existingSession?.UpdatableTo ?? expiresAt);
        }

        public async Task UpdateAsync(Session session, CancellationToken cancellationToken)
        {
            _dbContext.Sessions.Update(session);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _sessionsCache.Update(session.Id, true, session.UpdatableTo);
        }
    }
}