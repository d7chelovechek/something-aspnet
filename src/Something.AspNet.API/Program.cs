using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Scalar.AspNetCore;
using Something.AspNet.API.AuthenticationHandlers;
using Something.AspNet.API.BackgroundServices;
using Something.AspNet.API.Cache;
using Something.AspNet.API.Cache.Interfaces;
using Something.AspNet.API.ExceptionHandlers;
using Something.AspNet.API.Options;
using Something.AspNet.API.Requests;
using Something.AspNet.API.Services;
using Something.AspNet.API.Services.Interfaces;
using Something.AspNet.API.Services.Validators;
using Something.AspNet.Database.Extensions;
using Something.AspNet.Database.Models;

namespace Something.AspNet.API;

internal static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddControllers();

        builder.Services
            .AddAuthentication()
            .AddScheme<AuthenticationSchemeOptions, JwtAuthenticationHandler>(
                JwtAuthenticationHandler.SCHEME_NAME, 
                null);

        builder.Services.AddAuthorization();

        builder.Services.AddOptions<JwtOptions>().BindConfiguration(nameof(JwtOptions));

        builder.Services.AddDatabase();
        builder.Services.AddScoped<IUsersService, UsersService>();
        builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
    
        builder.Services.AddOpenApi();

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

        builder.Services.AddHostedService<DatabaseMigrationBackgroundService>();
        builder.Services.AddHostedService<DeleteExpiredSessionsBackgroundService>();

        builder.Services.AddTransient<IValidator<RegisterRequest>, RegisterRequestValidator>();

        builder.Services.AddScoped<IAccessTokenService, AccessTokenService>();
        builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        builder.Services.AddScoped<ISessionsService, SessionsService>();
        builder.Services.AddScoped<ISessionsCache, SessionsCache>();
        builder.Services.AddMemoryCache();

        builder.Services.AddSingleton(TimeProvider.System);

        var app = builder.Build();

        app.UseExceptionHandler(_ => { });
        app.MapOpenApi();
        app.MapScalarApiReference("/openapi");

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}