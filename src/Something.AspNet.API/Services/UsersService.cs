using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Something.AspNet.API.Exceptions;
using Something.AspNet.API.Requests;
using Something.AspNet.API.Services.Interfaces;
using Something.AspNet.Database;
using Something.AspNet.Database.Models;

namespace Something.AspNet.API.Services;

internal class UsersService(
    IApplicationDbContext dbContext,
    IPasswordHasher<User> passwordHasher,
    IValidator<RegisterRequest> registerValidator)
    : IUsersService
{
    private readonly IApplicationDbContext _dbContext = dbContext;
    private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;
    private readonly IValidator<RegisterRequest> _registerValidator = registerValidator;

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
        await _registerValidator.ValidateAndThrowAsync(request, cancellationToken);

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