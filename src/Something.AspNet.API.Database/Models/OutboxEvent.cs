namespace Something.AspNet.API.Database.Models;

public class OutboxEvent
{
    public Guid Id { get; set; }

    public string Payload { get; set; } = string.Empty;

    public bool Sent { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
}