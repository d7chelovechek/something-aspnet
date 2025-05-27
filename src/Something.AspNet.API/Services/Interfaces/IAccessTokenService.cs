using Something.AspNet.API.Database.Models;
using Something.AspNet.API.Models;

namespace Something.AspNet.API.Services.Interfaces;

public interface IAccessTokenService
{
    public string Create(Session session);

    public SessionPrincipal? Validate(string token);
}