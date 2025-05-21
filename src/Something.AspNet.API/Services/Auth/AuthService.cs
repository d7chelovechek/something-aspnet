using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Something.AspNet.API.Options;
using Something.AspNet.API.Requests;
using Something.AspNet.API.Responses;
using Something.AspNet.API.Services.Auth.Interfaces;
using Something.AspNet.API.Services.Auth.Exceptions;
using Something.AspNet.Database;
using Something.AspNet.Database.Models;

namespace Something.AspNet.API.Services.Auth;

internal class AuthService(
    IApplicationDbContext dbContext,
    IPasswordHasher<User> passwordHasher,
    IOptions<JwtOptions> jwtOptions,
    IAccessTokenManagementService accessTokenService,
    IRefreshTokenManagementService refreshTokenService,
    IValidator<RegisterRequest> registerValidator)
    : IAuthService
{
    private readonly IApplicationDbContext _dbContext = dbContext;
    private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;
    private readonly IAccessTokenManagementService _accessTokenService = accessTokenService;
    private readonly IRefreshTokenManagementService _refreshTokenService = refreshTokenService;
    private readonly IValidator<RegisterRequest> _registerValidator = registerValidator;

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

        var now = DateTime.UtcNow;

        var session = new Session()
        {
            UserId = existingUser.Id,
            ExpiredAt = now.AddMinutes(_jwtOptions.SessionLifetime)
        };

        await _dbContext.Sessions.AddAsync(session, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var accessToken = _accessTokenService.CreateToken(
            existingUser.Id, 
            session.Id, 
            now);

        var refreshToken = _refreshTokenService.CreateToken(
            existingUser.Id,
            session.Id,
            now);

        return new LoginResponse(accessToken, refreshToken);
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