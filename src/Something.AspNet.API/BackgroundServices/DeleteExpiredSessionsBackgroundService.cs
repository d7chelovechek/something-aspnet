using Microsoft.EntityFrameworkCore;
using Something.AspNet.Database;

namespace Something.AspNet.API.BackgroundServices
{
    public class DeleteExpiredSessionsBackgroundService(
        IServiceScopeFactory scopeFactory)
        : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.UtcNow;

                using (IServiceScope scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

                    await dbContext.Sessions
                        .Where(s => now > s.ExpiredAt)
                        .ExecuteDeleteAsync(stoppingToken);
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}