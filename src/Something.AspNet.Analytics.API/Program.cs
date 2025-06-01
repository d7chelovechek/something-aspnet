using Scalar.AspNetCore;
using Something.AspNet.Analytics.API.Database.Extensions;
using Something.AspNet.Analytics.API.Extensions;

namespace Something.AspNet.Analytics.API;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services
            .AddDatabase()
            .AddBackgroundServices()
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
