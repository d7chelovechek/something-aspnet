namespace Something.AspNet.Auth.API.Cache.Interfaces;

public interface ISessionsCache
{
    public bool Update(Guid sessionId, bool isValid, DateTimeOffset expiresAt);

    public bool? Get(Guid sessionId);

    public void Remove(Guid sessionId);
}