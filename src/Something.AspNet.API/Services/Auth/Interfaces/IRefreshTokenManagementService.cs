using Something.AspNet.API.Responses;

namespace Something.AspNet.API.Services.Auth.Interfaces;

public interface IRefreshTokenManagementService
{
    public TokenResponse CreateToken(Guid userId, Guid sessionId, DateTime now);
}