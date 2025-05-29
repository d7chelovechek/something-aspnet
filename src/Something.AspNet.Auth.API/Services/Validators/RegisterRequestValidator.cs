using FluentValidation;
using Something.AspNet.Auth.API.Requests;

namespace Something.AspNet.Auth.API.Services.Validators;

internal class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(r => r.Name).NotNull().NotEmpty().MaximumLength(32);
        RuleFor(r => r.Password).NotNull().NotEmpty().MinimumLength(8).MaximumLength(32);
    }
}