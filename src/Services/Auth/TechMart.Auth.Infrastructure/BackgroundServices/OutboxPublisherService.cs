using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TechMart.Auth.Application.Abstractions.Events;
using TechMart.Auth.Domain.Primitives;
using TechMart.Auth.Domain.Roles.Events;
using TechMart.Auth.Domain.Users.Events;
using TechMart.Auth.Infrastructure.Events;

namespace TechMart.Auth.Infrastructure.BackgroundServices;

public class OutboxPublisherService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxPublisherService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(30);
    private readonly bool _isEnabled;

    public OutboxPublisherService(
        IServiceProvider serviceProvider,
        ILogger<OutboxPublisherService> logger
    )
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        // Check if all required dependencies are available
        using var scope = serviceProvider.CreateScope();
        var outboxRepository = scope.ServiceProvider.GetService<IOutboxRepository>();
        var kafkaProducer = scope.ServiceProvider.GetService<IKafkaProducer>();
        var eventSerializer = scope.ServiceProvider.GetService<IEventSerializer>();

        _isEnabled = outboxRepository != null && kafkaProducer != null && eventSerializer != null;

        if (!_isEnabled)
        {
            _logger.LogWarning(
                "Outbox Publisher Service disabled - missing dependencies (Kafka or other services)"
            );
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_isEnabled)
        {
            _logger.LogInformation("Outbox Publisher Service is disabled, exiting gracefully");
            return;
        }

        _logger.LogInformation("Outbox Publisher Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessOutboxMessagesAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation is requested
                _logger.LogInformation("Outbox Publisher Service is shutting down");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing outbox messages");
            }

            try
            {
                await Task.Delay(_interval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation is requested
                _logger.LogInformation("Outbox Publisher Service delay interrupted, shutting down");
                break;
            }
        }

        _logger.LogInformation("Outbox Publisher Service stopped");
    }

    private async Task ProcessOutboxMessagesAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var outboxRepository = scope.ServiceProvider.GetService<IOutboxRepository>();
        var kafkaProducer = scope.ServiceProvider.GetService<IKafkaProducer>();
        var eventSerializer = scope.ServiceProvider.GetService<IEventSerializer>();

        // Double-check dependencies are still available
        if (outboxRepository == null || kafkaProducer == null || eventSerializer == null)
        {
            _logger.LogWarning("Required dependencies not available, skipping outbox processing");
            return;
        }

        var unprocessedMessages = await outboxRepository.GetUnprocessedMessagesAsync(
            100,
            cancellationToken
        );

        if (!unprocessedMessages.Any())
        {
            _logger.LogDebug("No unprocessed outbox messages found");
            return;
        }

        _logger.LogDebug("Processing {Count} outbox messages", unprocessedMessages.Count());

        foreach (var message in unprocessedMessages)
        {
            // Check for cancellation before processing each message
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                // Determine topic based on event type
                var topic = GetTopicName(message.EventType);

                // Deserialize and publish
                var domainEvent = eventSerializer.Deserialize(message.EventType, message.EventData);
                await kafkaProducer.PublishAsync(
                    topic,
                    message.EventId.ToString(),
                    domainEvent,
                    cancellationToken
                );

                // Mark as processed
                message.MarkAsProcessed();
                await outboxRepository.UpdateAsync(message, cancellationToken);

                _logger.LogDebug("Outbox message {MessageId} processed successfully", message.Id);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation(
                    "Outbox processing canceled for message {MessageId}",
                    message.Id
                );
                throw; // Re-throw to stop the processing loop
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process outbox message {MessageId}", message.Id);

                message.MarkAsFailed(ex.Message);
                await outboxRepository.UpdateAsync(message, cancellationToken);

                // Delete message if max retries exceeded
                if (!message.ShouldRetry)
                {
                    _logger.LogWarning(
                        "Outbox message {MessageId} exceeded max retries, deleting",
                        message.Id
                    );
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
            _ => "auth.events.unknown",
        };
    }
}
