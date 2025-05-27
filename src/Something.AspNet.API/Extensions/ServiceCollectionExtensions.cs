using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Something.AspNet.API.AuthenticationHandlers;
using Something.AspNet.API.BackgroundServices;
using Something.AspNet.API.Cache;
using Something.AspNet.API.Cache.Interfaces;
using Something.AspNet.API.Database.Models;
using Something.AspNet.API.Models.Options;
using Something.AspNet.API.Requests;
using Something.AspNet.API.Services;
using Something.AspNet.API.Services.Interfaces;
using Something.AspNet.API.Services.Validators;

namespace Something.AspNet.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJwtAuth(this IServiceCollection services)
    {
        services
            .AddAuthentication()
            .AddScheme<AuthenticationSchemeOptions, JwtAuthenticationHandler>(
                JwtAuthenticationHandler.SCHEME_NAME,
                null);

        services.AddAuthorization();

        return services;
    }

    public static IServiceCollection AddBackgroundServices(this IServiceCollection services)
    {
        services.AddHostedService<DatabaseMigrationBackgroundService>();
        services.AddHostedService<DeleteExpiredSessionsBackgroundService>();

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddTransient<IValidator<RegisterRequest>, RegisterRequestValidator>();
        services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddScoped<IUsersService, UsersService>();

        services.AddMemoryCache();
        services.AddScoped<ISessionsService, SessionsService>();
        services.AddScoped<ISessionsCache, SessionsCache>();

        services.AddScoped<IAccessTokenService, AccessTokenService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();

        return services;
    }

    public static IServiceCollection AddBindOptions(this IServiceCollection services)
    {
        services.AddOptions<JwtOptions>().BindConfiguration(nameof(JwtOptions));

        return services;
    }
}