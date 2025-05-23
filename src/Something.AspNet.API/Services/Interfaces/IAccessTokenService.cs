using Something.AspNet.API.Models;
using Something.AspNet.Database.Models;

namespace Something.AspNet.API.Services.Interfaces;

public interface IAccessTokenService
{
    public string Create(Session session);

    public SessionPrincipal? Validate(string token);
}