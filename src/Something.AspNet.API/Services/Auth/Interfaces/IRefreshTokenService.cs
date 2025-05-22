using Something.AspNet.API.Responses;
using Something.AspNet.Database.Models;

namespace Something.AspNet.API.Services.Auth.Interfaces;

public interface IRefreshTokenService
{
    public string CreateToken(Session session);
}