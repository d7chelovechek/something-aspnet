using Microsoft.Extensions.Caching.Memory;
using Something.AspNet.Auth.API.Cache.Interfaces;

namespace Something.AspNet.Auth.API.Cache;

internal class SessionsCache(IMemoryCache memoryCache) : ISessionsCache
{
    private readonly IMemoryCache _memoryCache = memoryCache;

    public bool Update(Guid sessionId, bool isValid, DateTimeOffset expiresAt)
    {
        _memoryCache.Remove(sessionId);

        return _memoryCache.Set(sessionId, isValid, expiresAt);
    }

    public bool? Get(Guid sessionId)
    {
        if (_memoryCache.TryGetValue(sessionId, out bool isValid))
        {
            return isValid;
        }

        return null;
    }

    public void Remove(Guid sessionId)
    {
        _memoryCache.Remove(sessionId);
    }
}