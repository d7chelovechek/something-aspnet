using Microsoft.Extensions.Options;
using Something.AspNet.Auth.API.Models.Options;
using Something.AspNet.Auth.API.Services.Interfaces;

namespace Something.AspNet.Auth.API.BackgroundServices;

internal class DeleteExpiredSessionsBackgroundService(
    IServiceScopeFactory scopeFactory,
    IOptions<JwtOptions> jwtOptions)
    : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (IServiceScope scope = _scopeFactory.CreateScope())
            {
                var sessionsService = scope.ServiceProvider.GetRequiredService<ISessionsService>();

                await sessionsService.RemoveExpiredAsync(stoppingToken);
            }

            await Task.Delay(
                TimeSpan.FromMinutes(_jwtOptions.SessionLifetimeInMinutes), 
                stoppingToken);
        }
    }
}