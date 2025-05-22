using Something.AspNet.API.Responses;
using Something.AspNet.Database.Models;
using System.Security.Claims;

namespace Something.AspNet.API.Services.Auth.Interfaces;

public interface IAccessTokenService
{
    public string CreateToken(Session session);

    public ClaimsPrincipal? ValidateToken(string token);
}