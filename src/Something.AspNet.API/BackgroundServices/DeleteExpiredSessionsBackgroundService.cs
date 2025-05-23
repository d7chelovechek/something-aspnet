using Microsoft.EntityFrameworkCore;
using Something.AspNet.Database;

namespace Something.AspNet.API.BackgroundServices;

internal class DeleteExpiredSessionsBackgroundService(
    IServiceScopeFactory scopeFactory,
    TimeProvider timeProvider)
    : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly TimeProvider _timeProvider = timeProvider;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (IServiceScope scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

                await dbContext.Sessions
                    .Where(s => _timeProvider.GetUtcNow() > s.SessionExpiresAt)
                    .ExecuteDeleteAsync(stoppingToken);
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}