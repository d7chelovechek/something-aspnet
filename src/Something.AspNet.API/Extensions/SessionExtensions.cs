using Something.AspNet.API.Database.Models;
using Something.AspNet.MessageBroker.Models;
using System.Text.Json;

namespace Something.AspNet.API.Extensions;

internal static class SessionExtensions
{
    public static OutboxEvent ToOutboxEvent(
        this Session session,
        DateTimeOffset now,
        SessionUpdatedEventType eventType)
    {
        var sessionPayload = new SessionUpdatedEvent()
        {
            SessionId = session.Id,
            UserId = session.UserId,
            EventType = eventType,
            UpdatedAt = now
        };

        return new OutboxEvent()
        {
            Payload = JsonSerializer.Serialize(sessionPayload),
            Sent = false,
            CreatedAt = now
        };
    }
}