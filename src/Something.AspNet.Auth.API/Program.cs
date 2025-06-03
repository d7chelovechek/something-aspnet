using Scalar.AspNetCore;
using Something.AspNet.Auth.API.Database.Extensions;
using Something.AspNet.Auth.API.ExceptionHandlers;
using Something.AspNet.Auth.API.Extensions;

namespace Something.AspNet.Auth.API;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
    
        builder.Services
            .AddJwtAuth()
            .AddBindOptions()
            .AddDatabase()
            .AddValidators()
            .AddServices()
            .AddBackgroundServices()
            .AddExceptionHandler<GlobalExceptionHandler>()
            .AddSingleton(TimeProvider.System)
            .AddOpenApi()
            .AddControllers();

        var app = builder.Build();

        app.UseExceptionHandler(_ => { });
        app.MapOpenApi();
        app.MapScalarApiReference("/openapi");

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        await app.RunAsync();
    }
}