using System.Text.Json.Serialization;

namespace Something.AspNet.MessageBrokers.Models;

public class SessionUpdatedEvent
{
    [JsonIgnore]
    public const string QUEUE_NAME = nameof(SessionUpdatedEvent);

    public Guid SessionId { get; set; }

    public Guid UserId { get; set; }

    public SessionUpdatedEventType EventType { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}