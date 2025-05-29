using Something.AspNet.Auth.API.Database;

namespace Something.AspNet.Auth.API.BackgroundServices;

internal class DatabaseMigrationBackgroundService(
    IServiceScopeFactory scopeFactory) : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using IServiceScope createdScope = _scopeFactory.CreateScope();

        var dbContext = createdScope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

        await dbContext.MigrateDatabaseAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}