using Something.AspNet.Database.Models;
using System.Security.Claims;

namespace Something.AspNet.API.Services.Interfaces;

public interface IAccessTokenService
{
    public string CreateToken(Session session);

    public ClaimsPrincipal? ValidateToken(string token);
}