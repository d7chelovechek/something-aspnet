using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Something.AspNet.Auth.API.Exceptions;
using Something.AspNet.Auth.API.Responses;
using System.Net;

namespace Something.AspNet.Auth.API.ExceptionHandlers;

internal record HandledException(HttpStatusCode StatusCode, ErrorsResponse ErrorsResponse);

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
            AuthenticatedSessionInvalidException => new HandledException(
                HttpStatusCode.Unauthorized, 
                new ErrorsResponse([exception.Message])),

            UserAlreadyExistsException => new HandledException(
                HttpStatusCode.Conflict, 
                new ErrorsResponse([exception.Message])),

            ValidationException => new HandledException(
                HttpStatusCode.UnprocessableEntity, 
                new ErrorsResponse(((ValidationException)exception).Errors.Select(e => e.ErrorMessage))),

            SessionNotFoundException => new HandledException(
                HttpStatusCode.NotFound, 
                new ErrorsResponse([exception.Message])),

            _ => new HandledException(
                HttpStatusCode.BadRequest, 
                new ErrorsResponse([exception.Message]))
        };

        httpContext.Response.StatusCode = (int)handledException.StatusCode;
        await httpContext.Response.WriteAsJsonAsync(handledException.ErrorsResponse, cancellationToken);

        return true;
    }
}