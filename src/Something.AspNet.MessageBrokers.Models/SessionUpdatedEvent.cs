namespace Something.AspNet.MessageBrokers.Models;

public class SessionUpdatedEvent
{
    public Guid SessionId { get; set; }

    public Guid UserId { get; set; }

    public SessionUpdatedEventType EventType { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}