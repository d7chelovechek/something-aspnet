using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Something.AspNet.API.Exceptions;
using Something.AspNet.API.Extensions;
using Something.AspNet.API.Requests;
using Something.AspNet.API.Responses;
using Something.AspNet.API.Services.Interfaces;
using Something.AspNet.Database;
using Something.AspNet.Database.Models;
using System.Security.Claims;

namespace Something.AspNet.API.Services;

internal class IdentityService(
    IApplicationDbContext dbContext,
    IPasswordHasher<User> passwordHasher,
    IAccessTokenService accessTokenService,
    IRefreshTokenService refreshTokenService,
    ISessionsService sessionsService,
    IValidator<RegisterRequest> registerValidator,
    TimeProvider timeProvider)
    : IIdentityService
{
    private readonly IApplicationDbContext _dbContext = dbContext;
    private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;
    private readonly IAccessTokenService _accessTokenService = accessTokenService;
    private readonly IRefreshTokenService _refreshTokenService = refreshTokenService;
    private readonly IValidator<RegisterRequest> _registerValidator = registerValidator;
    private readonly ISessionsService _sessionsService = sessionsService;
    private readonly TimeProvider _timeProvider = timeProvider;

    public async Task<LoginResponse> LoginAsync(
        LoginRequest request, 
        CancellationToken cancellationToken)
    {
        var existingUser =
            await _dbContext.Users.SingleOrDefaultAsync(
                r => r.Name.Equals(request.Name),
                cancellationToken) 
            ?? throw new CredentialsIncorrectException();

        var passwordValidationResult = 
            _passwordHasher.VerifyHashedPassword(
                existingUser, 
                existingUser.PasswordHash, 
                request.Password);

        if (passwordValidationResult is PasswordVerificationResult.Failed)
        {
            throw new CredentialsIncorrectException();
        }

        var session = await _sessionsService.CreateAsync(existingUser.Id, cancellationToken);

        return new LoginResponse(
            _accessTokenService.CreateToken(session),
            _refreshTokenService.CreateToken(session),
            session.AccessTokenExpiresAt.ToUnixTimeSeconds());
    }

    public async Task LogoutAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        await _sessionsService.RemoveAsync(sessionId, cancellationToken);
    }

    public async Task<RefreshResponse> RefreshAsync(
        RefreshRequest request, 
        CancellationToken cancellationToken)
    {
        if (_refreshTokenService.ValidateToken(request.RefreshToken)
                is not ClaimsPrincipal principal)
        {
            throw new TokenInvalidException();
        }

        Session? session = 
            await _dbContext.Sessions.SingleOrDefaultAsync(
                s => s.Id.Equals(principal.GetSessionId()), 
                cancellationToken)
            ?? throw new SessionExpiredException();

        if (_timeProvider.GetUtcNow() > session.SessionExpiresAt)
        {
            await _sessionsService.RemoveAsync(session.Id, cancellationToken);

            throw new SessionExpiredException();
        }

        if (!session.JwtId.Equals(principal.GetJwtId()))
        {
            throw new TokenInvalidException();
        }

        await _sessionsService.RefreshAsync(session, cancellationToken);

        return new RefreshResponse(
            _accessTokenService.CreateToken(session),
            _refreshTokenService.CreateToken(session),
            session.AccessTokenExpiresAt.ToUnixTimeSeconds());
    }

    public async Task RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var existingUser = 
            await _dbContext.Users.SingleOrDefaultAsync(
                r => r.Name.Equals(request.Name), 
                cancellationToken);

        if (existingUser is not null)
        {
            throw new UserAlreadyExistsException();
        }

        await _registerValidator.ValidateAndThrowAsync(request, cancellationToken);

        var newUser = new User()
        {
            Name = request.Name
        };

        newUser.PasswordHash = _passwordHasher.HashPassword(newUser, request.Password);

        await _dbContext.Users.AddAsync(newUser, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}