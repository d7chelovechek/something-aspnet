using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Something.AspNet.API.BackgroundServices;
using Something.AspNet.API.ExceptionHandlers;
using Something.AspNet.API.Options;
using Something.AspNet.API.Options.Configurations;
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
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer();

        builder.Services.AddOptions<JwtOptions>().BindConfiguration(nameof(JwtOptions));
        builder.Services.ConfigureOptions<ConfigureJwtBearerOptions>();

        builder.Services.AddDatabase();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
    
        builder.Services.AddOpenApi();

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

        builder.Services.AddHostedService<DatabaseMigrationBackgroundService>();

        builder.Services.AddTransient<IValidator<RegisterRequest>, RegisterRequestValidator>();

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