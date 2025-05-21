namespace Something.AspNet.Database.Models;

public class Session
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public DateTime ExpiredAt { get; set; }
}