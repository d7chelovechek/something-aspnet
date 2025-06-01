using RabbitMQ.Client;
using Something.AspNet.Analytics.API.BackgroundsServices;

namespace Something.AspNet.Analytics.API.Extensions;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBackgroundServices(this IServiceCollection services)
    {
        services.AddHostedService<DatabaseMigrationBackgroundService>();

        services.AddSingleton<IConnectionFactory, ConnectionFactory>();
        services.AddHostedService<ReceiveOutboxEventsBackgroundService>();

        return services;
    }
}