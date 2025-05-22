using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Something.AspNet.API.Cache.Interfaces;
using Something.AspNet.API.Options;
using Something.AspNet.API.Services.Interfaces;
using Something.AspNet.Database;
using Something.AspNet.Database.Models;

namespace Something.AspNet.API.Services;

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
            JwtId = Guid.NewGuid(),
            TokensUpdatedAt = now,
            SessionExpiresAt = now.AddMinutes(_jwtOptions.SessionLifetimeInMinutes),
            AccessTokenExpiresAt = now.AddMinutes(_jwtOptions.AccessTokenLifetimeInMinutes),
            RefreshTokenExpiresAt = now.AddMinutes(_jwtOptions.RefreshTokenLifetimeInMinutes)
        };

        await UpdateAsync(session, cancellationToken);

        return session;
    }

    public async Task<bool> IsValidAsync(Guid sessionId, CancellationToken cancellationToken)
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
            existingSession?.AccessTokenExpiresAt ??
            _timeProvider
                .GetUtcNow()
                .AddMinutes(_jwtOptions.AccessTokenLifetimeInMinutes));
    }

    public async Task UpdateAsync(Session session, CancellationToken cancellationToken)
    {
        _dbContext.Sessions.Update(session);
        var saveTask = _dbContext.SaveChangesAsync(cancellationToken);

        _sessionsCache.Update(session.Id, true, session.AccessTokenExpiresAt);

        await saveTask;
    }

    public async Task RemoveAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        var deleteTask = _dbContext.Sessions
            .Where(s => s.Id.Equals(sessionId))
            .ExecuteDeleteAsync(cancellationToken);

        _sessionsCache.Remove(sessionId);

        await deleteTask;
    }

    public async Task RefreshAsync(Session session, CancellationToken cancellationToken)
    {
        var now = _timeProvider.GetUtcNow();

        session.JwtId = Guid.NewGuid();
        session.TokensUpdatedAt = now;
        session.AccessTokenExpiresAt = now.AddMinutes(_jwtOptions.AccessTokenLifetimeInMinutes);
        session.RefreshTokenExpiresAt = now.AddMinutes(_jwtOptions.RefreshTokenLifetimeInMinutes);

        await UpdateAsync(session, cancellationToken);
    }
}