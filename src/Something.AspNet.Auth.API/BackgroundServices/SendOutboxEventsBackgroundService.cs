using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Something.AspNet.Auth.API.Database;
using Something.AspNet.Auth.API.Database.Models;
using System.Text;

namespace Something.AspNet.Auth.API.BackgroundServices;

internal class SendOutboxEventsBackgroundService(
    IServiceScopeFactory scopeFactory, 
    ILogger<SendOutboxEventsBackgroundService> logger,
    IConnectionFactory connectionFactory)
    : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly ILogger<SendOutboxEventsBackgroundService> _logger = logger;
    private readonly IConnection _connection =
        connectionFactory.CreateConnectionAsync().GetAwaiter().GetResult();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (IServiceScope scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

                    List<OutboxEvent> events =
                        await dbContext.OutboxEvents
                            .Where(e => !e.Sent)
                            .ToListAsync(stoppingToken);

                    string queueName = nameof(OutboxEvent);
                    
                    using var channel = await _connection.CreateChannelAsync(null, stoppingToken);
                    await channel.QueueDeclareAsync(
                        queueName, false, false, false, null, false, false, stoppingToken);

                    foreach (var @event in events)
                    {
                        try
                        {
                            await channel.BasicPublishAsync(
                                string.Empty, 
                                queueName, 
                                Encoding.UTF8.GetBytes(@event.Payload),
                                stoppingToken);
                            
                            @event.Sent = true;

                            dbContext.OutboxEvents.Update(@event);
                            await dbContext.SaveChangesAsync(stoppingToken);
                        }
                        catch (OperationCanceledException)
                        {
                            throw;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "An error occurred while processing the outbox event.");
                        }
                    }
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
        finally
        {
            _connection?.Dispose();
        }
    }
}