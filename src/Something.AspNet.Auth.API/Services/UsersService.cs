using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Something.AspNet.Auth.API.Database;
using Something.AspNet.Auth.API.Database.Models;
using Something.AspNet.Auth.API.Exceptions;
using Something.AspNet.Auth.API.Requests;
using Something.AspNet.Auth.API.Services.Interfaces;

namespace Something.AspNet.Auth.API.Services;

internal class UsersService(IApplicationDbContext dbContext, IPasswordHasher<User> passwordHasher)
    : IUsersService
{
    private readonly IApplicationDbContext _dbContext = dbContext;
    private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;

    public async Task<Guid> LoginAsync(
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

        return existingUser.Id;
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

        var newUser = new User()
        {
            Name = request.Name
        };

        newUser.PasswordHash = _passwordHasher.HashPassword(newUser, request.Password);

        await _dbContext.Users.AddAsync(newUser, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}