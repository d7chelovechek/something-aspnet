using Something.AspNet.Database.Models;

namespace Something.AspNet.API.Services.Cache.Interfaces
{
    public interface ISessionsCache
    {
        public bool Update(Guid sessionId, bool isValid, DateTimeOffset expiresAt);

        public bool? Get(Guid sessionId);
    }
}