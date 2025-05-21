using Something.AspNet.API.Responses;

namespace Something.AspNet.API.Services.Auth.Interfaces;

public interface IAccessTokenManagementService
{
    public TokenResponse CreateToken(Guid userId, Guid sessionId, DateTime now);
}