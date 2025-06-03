using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Something.AspNet.Auth.API.Database;
using Something.AspNet.Auth.API.Database.Models;
using Something.AspNet.MessageBrokers.Models;
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
            string queueName = SessionUpdatedEvent.QUEUE_NAME;

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

                using IServiceScope scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

                List<OutboxEvent> events =
                    await dbContext.OutboxEvents
                        .Where(e => !e.Sent)
                        .ToListAsync(stoppingToken);

                if (events.Count is 0)
                {
                    continue;
                }

                using var channel = await _connection.CreateChannelAsync(null, stoppingToken);
                await channel.QueueDeclareAsync(
                    queueName, false, false, false, null, false, false, stoppingToken);

                foreach (var @event in events)
                {
                    try
                    {
                        stoppingToken.ThrowIfCancellationRequested();

                        var properties = new BasicProperties()
                        {
                            MessageId = @event.Id.ToString()
                        };

                        await channel.BasicPublishAsync(
                            string.Empty,
                            queueName,
                            false,
                            properties,
                            Encoding.UTF8.GetBytes(@event.Payload),
                            stoppingToken);

                        @event.Sent = true;

                        dbContext.OutboxEvents.Update(@event);
                        await dbContext.SaveChangesAsync(stoppingToken);

                        _logger.LogInformation("Sent event with Id: {MessageId}", @event.Id);
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
        }
        finally
        {
            _connection?.Dispose();
        }
    }
}