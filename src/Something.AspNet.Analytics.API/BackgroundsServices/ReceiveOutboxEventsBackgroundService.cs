using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Something.AspNet.Analytics.API.Database;
using Something.AspNet.Analytics.API.Database.Models;
using Something.AspNet.MessageBrokers.Models;
using System.Data;
using System.Text;
using System.Text.Json;

namespace Something.AspNet.Analytics.API.BackgroundsServices;

public class ReceiveOutboxEventsBackgroundService(
    IServiceScopeFactory scopeFactory,
    ILogger<ReceiveOutboxEventsBackgroundService> logger,
    IConnectionFactory connectionFactory)
    : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly ILogger<ReceiveOutboxEventsBackgroundService> _logger = logger;
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

                using var channel = await _connection.CreateChannelAsync(null, stoppingToken);
                var queueDeclare = await channel.QueueDeclareAsync(
                    queueName, false, false, false, null, false, false, stoppingToken);

                for (var i = 0; i < queueDeclare.MessageCount; i++)
                {
                    stoppingToken.ThrowIfCancellationRequested();

                    var @event = await channel.BasicGetAsync(queueName, false, stoppingToken);

                    if (!Guid.TryParse(@event!.BasicProperties.MessageId, out Guid messageId))
                    {
                        await channel.BasicAckAsync(@event.DeliveryTag, false, stoppingToken);

                        continue;
                    }

                    _logger.LogInformation("Received event with Id: {MessageId}", messageId);

                    var exists = await dbContext.OutboxEvents
                        .Where(e => e.Id.Equals(messageId))
                        .AnyAsync(stoppingToken);

                    if (exists)
                    {
                        await channel.BasicAckAsync(@event.DeliveryTag, false, stoppingToken);

                        continue;
                    }

                    var json = Encoding.UTF8.GetString(@event.Body.ToArray());
                    var sessionUpdatedEvent = JsonSerializer.Deserialize<SessionUpdatedEvent>(json);

                    if (sessionUpdatedEvent is null)
                    {
                        await channel.BasicAckAsync(@event.DeliveryTag, false, stoppingToken);

                        continue;
                    }

                    using var transaction = await dbContext.BeginTransactionAsync(
                        IsolationLevel.RepeatableRead, 
                        stoppingToken);

                    var sessionsUpdate = new SessionUpdate()
                    {
                        SessionId = sessionUpdatedEvent.SessionId,
                        UserId = sessionUpdatedEvent.UserId,
                        UpdatedAt = sessionUpdatedEvent.UpdatedAt,
                        Type = (SessionUpdateType)sessionUpdatedEvent.EventType
                    };

                    var outboxEvent = new OutboxEvent()
                    {
                        Id = messageId
                    };

                    dbContext.SessionsUpdates.Add(sessionsUpdate);
                    dbContext.OutboxEvents.Add(outboxEvent);

                    await dbContext.SaveChangesAsync(stoppingToken);
                    await transaction.CommitAsync(stoppingToken);

                    await channel.BasicAckAsync(@event.DeliveryTag, false, stoppingToken);
                }
            }
        }
        finally
        {
            _connection?.Dispose();
        }
    }
}