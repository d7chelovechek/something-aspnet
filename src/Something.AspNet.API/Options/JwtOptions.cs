namespace Something.AspNet.API.Options;

internal class JwtOptions
{
    public string AccessTokenKey { get; set; } = string.Empty;
    public string RefreshTokenKey { get; set; } = string.Empty;

    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;

    public int AccessTokenLifetimeInMinutes { get; set; }
    public int RefreshTokenLifetimeInMinutes { get; set; }
    public int SessionLifetimeInMinutes { get; set; }
}