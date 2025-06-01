namespace Something.AspNet.Analytics.API.Database.Models;

public class SessionUpdate
{
    public Guid Id { get; set; }

    public Guid SessionId { get; set; }

    public Guid UserId { get; set; }

    public SessionUpdateType Type { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}