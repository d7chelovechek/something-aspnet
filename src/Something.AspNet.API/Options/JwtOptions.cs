namespace Something.AspNet.API.Options
{
    internal class JwtOptions
    {
        public string AccessTokenKey { get; set; }
        public string RefreshTokenKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}