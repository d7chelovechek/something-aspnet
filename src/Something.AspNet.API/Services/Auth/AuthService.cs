using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Something.AspNet.API.Constants;
using Something.AspNet.API.Options;
using Something.AspNet.API.Requests;
using Something.AspNet.API.Responses;
using Something.AspNet.API.Services.Auth.Exceptions;
using Something.AspNet.API.Services.Auth.Interfaces;
using Something.AspNet.Database;
using Something.AspNet.Database.Models;
using System.IdentityModel.Tokens.Jwt;

namespace Something.AspNet.API.Services.Auth;

internal class AuthService(
    IApplicationDbContext dbContext,
    IPasswordHasher<User> passwordHasher,
    IAccessTokenService accessTokenService,
    IRefreshTokenService refreshTokenService,
    ISessionsService sessionsService,
    IValidator<RegisterRequest> registerValidator)
    : IAuthService
{
    private readonly IApplicationDbContext _dbContext = dbContext;
    private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;
    private readonly IAccessTokenService _accessTokenService = accessTokenService;
    private readonly IRefreshTokenService _refreshTokenService = refreshTokenService;
    private readonly IValidator<RegisterRequest> _registerValidator = registerValidator;
    private readonly ISessionsService _sessionsService = sessionsService;

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
            _refreshTokenService.CreateToken(session));
    }

    public async Task LogoutAsync(string accessToken, CancellationToken cancellationToken)
    {
        //var securityToken = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
        //var claim = securityToken.Claims.Single(c => c.Type is JwtClaimTypes.SessionId);

        //await _dbContext.Sessions
        //    .Where(s => s.Id.Equals(Guid.Parse(claim.Value)))
        //    .ExecuteDeleteAsync(cancellationToken);
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