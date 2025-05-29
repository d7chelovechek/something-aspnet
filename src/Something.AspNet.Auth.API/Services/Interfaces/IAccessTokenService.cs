using Something.AspNet.Auth.API.Database.Models;
using Something.AspNet.Auth.API.Models;

namespace Something.AspNet.Auth.API.Services.Interfaces;

public interface IAccessTokenService
{
    public string Create(Session session);

    public SessionPrincipal? Validate(string token);
}