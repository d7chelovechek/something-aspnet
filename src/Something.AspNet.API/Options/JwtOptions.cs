namespace Something.AspNet.API.Options;

internal class JwtOptions
{
    public string AccessTokenKey { get; set; } = string.Empty;
    public string RefreshTokenKey { get; set; } = string.Empty;

    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;

    public int AccessTokenLifetime { get; set; }
    public int RefreshTokenLifetime { get; set; }
    public int SessionLifetime { get; set; }
}