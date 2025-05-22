namespace Something.AspNet.Database.Models;

public class Session
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public Guid JwtId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatableTo { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
}