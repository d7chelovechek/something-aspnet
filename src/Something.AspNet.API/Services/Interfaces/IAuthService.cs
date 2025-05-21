using FluentValidation;
using Something.AspNet.API.Requests;

namespace Something.AspNet.API.Services.Interfaces
{
    public interface IAuthService
    {
        public Task RegisterAsync(
            RegisterRequest request,
            IValidator<RegisterRequest> validator, 
            CancellationToken cancellationToken);
    }
}