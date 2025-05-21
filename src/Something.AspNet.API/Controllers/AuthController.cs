using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Something.AspNet.API.Requests;
using Something.AspNet.API.Services.Interfaces;

namespace Something.AspNet.API.Controllers
{
    [ApiController, Route("auth")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(
            RegisterRequest request,
            IValidator<RegisterRequest> validator, 
            CancellationToken cancellationToken)
        {
            await _authService.RegisterAsync(request, validator, cancellationToken);

            return Created();
        }
    }
}