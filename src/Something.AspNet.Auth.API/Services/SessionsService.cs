using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Something.AspNet.Auth.API.Exceptions;
using Something.AspNet.Auth.API.Cache.Interfaces;
using Something.AspNet.Auth.API.Database;
using Something.AspNet.Auth.API.Database.Models;
using Something.AspNet.Auth.API.Extensions;
using Something.AspNet.Auth.API.Models;
using Something.AspNet.Auth.API.Models.Options;
using Something.AspNet.Auth.API.Responses;
using Something.AspNet.Auth.API.Services.Interfaces;
using Something.AspNet.MessageBroker.Models;

namespace Something.AspNet.Auth.API.Services;

internal class SessionsService(
    IOptions<JwtOptions> jwtOptions,
    IApplicationDbContext dbContext,
    IAccessTokenService accessTokenService,
    IRefreshTokenService refreshTokenService,
    ISessionsCache sessionsCache,
    TimeProvider timeProvider)
    : ISessionsService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;
    private readonly IApplicationDbContext _dbContext = dbContext;
    private readonly ISessionsCache _sessionsCache = sessionsCache;
    private readonly TimeProvider _timeProvider = timeProvider;
    private readonly IAccessTokenService _accessTokenService = accessTokenService;
    private readonly IRefreshTokenService _refreshTokenService = refreshTokenService;

    private const System.Data.IsolationLevel ISOLATION_LEVEL = System.Data.IsolationLevel.RepeatableRead;

    public async Task<CreatedSessionResponse> CreateAsync(
        Guid userId, 
        CancellationToken cancellationToken)
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

        await AddAsync(session, cancellationToken);

        return new CreatedSessionResponse(
            _accessTokenService.Create(session),
            _refreshTokenService.Create(session),
            session.AccessTokenExpiresAt.ToUnixTimeSeconds());
    }

    public async Task<ActiveSessionsResponse> GetActiveAsync(
        Guid userId, 
        CancellationToken cancellationToken)
    {
        var now = _timeProvider.GetUtcNow();

        var activeSessions = 
            await _dbContext.Sessions
                .Where(s => s.UserId.Equals(userId) && now < s.SessionExpiresAt)
                .Select(s => s.Id)
                .ToListAsync(cancellationToken);

        return new ActiveSessionsResponse(activeSessions);
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

    public async Task<RefreshedSessionResponse> RefreshAsync(
        string refreshToken,
        CancellationToken cancellationToken)
    {
        if (_refreshTokenService.Validate(refreshToken)
                is not SessionPrincipal principal)
        {
            throw new TokenInvalidException();
        }

        Session? session =
            await _dbContext.Sessions.SingleOrDefaultAsync(
                s => s.Id.Equals(principal.SessionId),
                cancellationToken)
            ?? throw new SessionNotFoundException();

        if (_timeProvider.GetUtcNow() > session.SessionExpiresAt)
        {
            await RemoveAsync(SessionUpdatedEventType.Expired, cancellationToken, session);

            throw new SessionExpiredException();
        }

        if (!session.JwtId.Equals(principal.JwtId))
        {
            throw new TokenInvalidException();
        }

        var now = _timeProvider.GetUtcNow();

        session.JwtId = Guid.NewGuid();
        session.TokensUpdatedAt = now;
        session.AccessTokenExpiresAt = now.AddMinutes(_jwtOptions.AccessTokenLifetimeInMinutes);
        session.RefreshTokenExpiresAt = now.AddMinutes(_jwtOptions.RefreshTokenLifetimeInMinutes);

        await UpdateAsync(session, cancellationToken);

        return new RefreshedSessionResponse(
            _accessTokenService.Create(session),
            _refreshTokenService.Create(session),
            session.AccessTokenExpiresAt.ToUnixTimeSeconds());
    }

    public async Task RemoveExpiredAsync(CancellationToken cancellationToken)
    {
        var now = _timeProvider.GetUtcNow();

        Session[] sessions = 
            await _dbContext.Sessions
                .Where(s => now >= s.SessionExpiresAt)
                .ToArrayAsync(cancellationToken);

        await RemoveAsync(SessionUpdatedEventType.Expired, cancellationToken, sessions);
    }

    public async Task RemoveAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        Session? session =
            await _dbContext.Sessions.SingleOrDefaultAsync(
                s => s.Id.Equals(sessionId),
                cancellationToken)
            ?? throw new SessionExpiredException();

        await RemoveAsync(SessionUpdatedEventType.Finished, cancellationToken, session);
    }

    public async Task RemoveWithPrincipalCheckAsync(
        Guid sessionId, 
        SessionPrincipal principal, 
        CancellationToken cancellationToken)
    {
        if (sessionId.Equals(principal.SessionId))
        {
            throw new CannotRemoveCurrentSessionException();
        }

        Session? session =
            await _dbContext.Sessions.SingleOrDefaultAsync(
                s => s.Id.Equals(sessionId) && s.UserId.Equals(principal.UserId),
                cancellationToken)
            ?? throw new SessionNotFoundException();
    
        await RemoveAsync(SessionUpdatedEventType.Finished, cancellationToken, session);
    }

    private async Task AddAsync(Session session, CancellationToken cancellationToken)
    {
        using var transaction = 
            await _dbContext.BeginTransactionAsync(ISOLATION_LEVEL, cancellationToken);

        _dbContext.Sessions.Add(session);

        await _dbContext.SaveChangesAsync(cancellationToken);

        var outboxEvent =
            session.ToOutboxEvent(_timeProvider.GetUtcNow(), SessionUpdatedEventType.Created);

        _dbContext.OutboxEvents.Add(outboxEvent);

        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        _sessionsCache.Update(session.Id, true, session.AccessTokenExpiresAt);
    }

    private async Task UpdateAsync(Session session, CancellationToken cancellationToken)
    {
        using var transaction =
            await _dbContext.BeginTransactionAsync(ISOLATION_LEVEL, cancellationToken);

        var outboxEvent =
            session.ToOutboxEvent(_timeProvider.GetUtcNow(), SessionUpdatedEventType.Refreshed);

        _dbContext.Sessions.Update(session);
        _dbContext.OutboxEvents.Add(outboxEvent);

        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        _sessionsCache.Update(session.Id, true, session.AccessTokenExpiresAt);
    }

    private async Task RemoveAsync(
        SessionUpdatedEventType eventType,
        CancellationToken cancellationToken,
        params Session[] session)
    {
        using var transaction =
            await _dbContext.BeginTransactionAsync(ISOLATION_LEVEL, cancellationToken);

        var now = _timeProvider.GetUtcNow();

        _dbContext.Sessions.RemoveRange(session);
        _dbContext.OutboxEvents.AddRange(session.Select(s => s.ToOutboxEvent(now, eventType)));

        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        foreach (var sessionId in session.Select(s => s.Id))
        {
            _sessionsCache.Remove(sessionId);
        }
    }
}