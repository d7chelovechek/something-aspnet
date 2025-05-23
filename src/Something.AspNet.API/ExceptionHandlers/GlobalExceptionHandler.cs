using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Something.AspNet.API.Exceptions;
using System.Net;

namespace Something.AspNet.API.ExceptionHandlers;

internal record HandledException(HttpStatusCode StatusCode, IEnumerable<string> Errors);

internal class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        var handledException = exception switch
        {
            TokenInvalidException or 
            SessionExpiredException or 
            AuthorizedSessionInvalidException => new HandledException(
                HttpStatusCode.Unauthorized, 
                [exception.Message]),
            UserAlreadyExistsException => new HandledException(
                HttpStatusCode.Conflict, 
                [exception.Message]),
            ValidationException => new HandledException(
                HttpStatusCode.UnprocessableEntity, 
                ((ValidationException)exception).Errors.Select(e => e.ErrorMessage)),
            _ => new HandledException(
                HttpStatusCode.BadRequest, 
                [exception.Message])
        };

        httpContext.Response.StatusCode = (int)handledException.StatusCode;
        await httpContext.Response.WriteAsJsonAsync(new { handledException.Errors }, cancellationToken);

        return true;
    }
}