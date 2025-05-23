using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Something.AspNet.API.Cache.Interfaces;
using Something.AspNet.API.Exceptions;
using Something.AspNet.API.Models;
using Something.AspNet.API.Models.Options;
using Something.AspNet.API.Requests;
using Something.AspNet.API.Responses;
using Something.AspNet.API.Services.Interfaces;
using Something.AspNet.Database;
using Something.AspNet.Database.Models;

namespace Something.AspNet.API.Services;

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

        await UpdateAsync(session, cancellationToken);

        return new CreatedSessionResponse(
            _accessTokenService.Create(session),
            _refreshTokenService.Create(session),
            session.AccessTokenExpiresAt.ToUnixTimeSeconds());
    }

    public async Task<FoundSessionsResponse> GetAsync(
        Guid userId, 
        CancellationToken cancellationToken)
    {
        var now = _timeProvider.GetUtcNow();

        return new FoundSessionsResponse(
            await _dbContext.Sessions
                .Select(s =>
                    new FoundSession(
                        s.Id,
                        now > s.SessionExpiresAt))
                .ToListAsync(cancellationToken));
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
        RefreshSessionRequest request,
        CancellationToken cancellationToken)
    {
        if (_refreshTokenService.Validate(request.RefreshToken)
                is not SessionPrincipal principal)
        {
            throw new TokenInvalidException();
        }

        Session? session =
            await _dbContext.Sessions.SingleOrDefaultAsync(
                s => s.Id.Equals(principal.SessionId),
                cancellationToken)
            ?? throw new SessionExpiredException();

        if (_timeProvider.GetUtcNow() > session.SessionExpiresAt)
        {
            await RemoveAsync(session.Id, cancellationToken);

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

    public async Task RemoveAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        await _dbContext.Sessions
            .Where(s => s.Id.Equals(sessionId))
            .ExecuteDeleteAsync(cancellationToken);

        _sessionsCache.Remove(sessionId);
    }

    public async Task UpdateAsync(Session session, CancellationToken cancellationToken)
    {
        _dbContext.Sessions.Update(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _sessionsCache.Update(session.Id, true, session.AccessTokenExpiresAt);
    }
}