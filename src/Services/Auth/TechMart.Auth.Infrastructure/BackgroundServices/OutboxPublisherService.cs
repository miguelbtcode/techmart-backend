namespace TechMart.Auth.Infrastructure.BackgroundServices;

public class OutboxPublisherService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxPublisherService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(30);

    public OutboxPublisherService(IServiceProvider serviceProvider, ILogger<OutboxPublisherService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Outbox Publisher Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessOutboxMessagesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing outbox messages");
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }

    private async Task ProcessOutboxMessagesAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
        var kafkaProducer = scope.ServiceProvider.GetRequiredService<IKafkaProducer>();
        var eventSerializer = scope.ServiceProvider.GetRequiredService<IEventSerializer>();

        var unprocessedMessages = await outboxRepository.GetUnprocessedMessagesAsync(100, cancellationToken);

        foreach (var message in unprocessedMessages)
        {
            try
            {
                // Determine topic based on event type
                var topic = GetTopicName(message.EventType);

                // Deserialize and publish
                var domainEvent = eventSerializer.Deserialize(message.EventType, message.EventData);
                await kafkaProducer.PublishAsync(topic, message.EventId.ToString(), domainEvent, cancellationToken);

                // Mark as processed
                message.MarkAsProcessed();
                await outboxRepository.UpdateAsync(message, cancellationToken);

                _logger.LogDebug("Outbox message {MessageId} processed successfully", message.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process outbox message {MessageId}", message.Id);

                message.MarkAsFailed(ex.Message);
                await outboxRepository.UpdateAsync(message, cancellationToken);

                // Delete message if max retries exceeded
                if (!message.ShouldRetry)
                {
                    _logger.LogWarning("Outbox message {MessageId} exceeded max retries, deleting", message.Id);
                    await outboxRepository.DeleteAsync(message.Id, cancellationToken);
                }
            }
        }
    }

    private string GetTopicName(string eventType)
    {
        return eventType switch
        {
            nameof(UserRegisteredEvent) => "auth.user.registered",
            nameof(UserEmailConfirmedEvent) => "auth.user.email-confirmed",
            nameof(UserLoggedInEvent) => "auth.user.logged-in",
            nameof(UserPasswordChangedEvent) => "auth.user.password-changed",
            nameof(UserStatusChangedEvent) => "auth.user.status-changed",
            nameof(UserRoleAssignedEvent) => "auth.user.role-assigned",
            nameof(UserRoleRemovedEvent) => "auth.user.role-removed",
            _ => "auth.events.unknown"
        };
    }
}
