using Something.AspNet.Database.Models;

namespace Something.AspNet.API.Services.Interfaces;

public interface IRefreshTokenService
{
    public string CreateToken(Session session);
}