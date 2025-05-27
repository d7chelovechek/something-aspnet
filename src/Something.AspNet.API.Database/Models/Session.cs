namespace Something.AspNet.API.Database.Models;

public class Session
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public Guid JwtId { get; set; }

    public DateTimeOffset AccessTokenExpiresAt { get; set; }
    public DateTimeOffset RefreshTokenExpiresAt { get; set; }

    public DateTimeOffset TokensUpdatedAt { get; set; }
    public DateTimeOffset SessionExpiresAt { get; set; }
}