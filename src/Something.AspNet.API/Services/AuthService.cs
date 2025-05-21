using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Something.AspNet.API.Requests;
using Something.AspNet.API.Services.Exceptions;
using Something.AspNet.API.Services.Interfaces;
using Something.AspNet.Database;
using Something.AspNet.Database.Models;

namespace Something.AspNet.API.Services
{
    internal class AuthService(
        IApplicationDbContext dbContext,
        IPasswordHasher<User> passwordHasher) : IAuthService
    {
        private readonly IApplicationDbContext _dbContext = dbContext;
        private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;

        public async Task RegisterAsync(
            RegisterRequest request, 
            IValidator<RegisterRequest> validator, 
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

            await validator.ValidateAndThrowAsync(request, cancellationToken);

            var newUser = new User()
            {
                Name = request.Name
            };

            newUser.PasswordHash = _passwordHasher.HashPassword(newUser, request.Password);

            await _dbContext.Users.AddAsync(newUser, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}