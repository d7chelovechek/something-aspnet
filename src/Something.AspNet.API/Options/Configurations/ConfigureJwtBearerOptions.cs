using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Something.AspNet.API.Constants;
using System.Text;

namespace Something.AspNet.API.Options.Configurations;

internal class ConfigureJwtBearerOptions(IOptions<JwtOptions> jwtOptions) : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public void Configure(string? name, JwtBearerOptions options)
    {
        Configure(options);
    }

    public void Configure(JwtBearerOptions options)
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateLifetime = true,
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,

            ValidAudience = _jwtOptions.Audience,
            ValidIssuer = _jwtOptions.Issuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.AccessTokenKey)),

            ClockSkew = TimeSpan.FromSeconds(0)
        };
    }
}