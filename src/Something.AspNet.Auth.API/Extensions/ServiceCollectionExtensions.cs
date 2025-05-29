using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Something.AspNet.Auth.API.AuthenticationHandlers;
using Something.AspNet.Auth.API.BackgroundServices;
using Something.AspNet.Auth.API.Cache;
using Something.AspNet.Auth.API.Cache.Interfaces;
using Something.AspNet.Auth.API.Database.Models;
using Something.AspNet.Auth.API.Models.Options;
using Something.AspNet.Auth.API.Requests;
using Something.AspNet.Auth.API.Services;
using Something.AspNet.Auth.API.Services.Interfaces;
using Something.AspNet.Auth.API.Services.Validators;

namespace Something.AspNet.Auth.API.Extensions;

internal static class ServiceCollectionExtensions
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