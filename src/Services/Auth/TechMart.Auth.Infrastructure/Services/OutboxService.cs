using Microsoft.Extensions.Logging;
using TechMart.Auth.Application.Abstractions.Events;
using TechMart.Auth.Application.Contracts.Infrastructure;
using TechMart.Auth.Domain.Primitives;

namespace TechMart.Auth.Infrastructure.Services;

/// <summary>
/// Implementation of the Outbox Service for reliable event processing
/// </summary>
public sealed class OutboxService : IOutboxService
{
    private readonly IOutboxRepository _outboxRepository;
    private readonly IEventSerializer _eventSerializer;
    private readonly ILogger<OutboxService> _logger;

    public OutboxService(
        IOutboxRepository outboxRepository,
        IEventSerializer eventSerializer,
        ILogger<OutboxService> logger
    )
    {
        _outboxRepository = outboxRepository;
        _eventSerializer = eventSerializer;
        _logger = logger;
    }

    public async Task SaveDomainEventAsync(
        IDomainEvent domainEvent,
        CancellationToken cancellationToken = default
    )
    {
        await SaveDomainEventsAsync(new[] { domainEvent }, cancellationToken);
    }

    public async Task SaveDomainEventsAsync(
        IEnumerable<IDomainEvent> domainEvents,
        CancellationToken cancellationToken = default
    )
    {
        var events = domainEvents.ToList();
        if (!events.Any())
        {
            return;
        }

        _logger.LogDebug("Saving {EventCount} domain events to outbox", events.Count);

        foreach (var domainEvent in events)
        {
            try
            {
                var eventType = domainEvent.GetType().Name;
                var eventData = _eventSerializer.Serialize(domainEvent);

                var outboxMessage = new OutboxMessage(domainEvent, eventType, eventData);
                await _outboxRepository.AddAsync(outboxMessage, cancellationToken);

                _logger.LogDebug(
                    "Domain event {EventType} with ID {EventId} saved to outbox",
                    eventType,
                    domainEvent.Id
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to save domain event {EventType} to outbox",
                    domainEvent.GetType().Name
                );
                throw;
            }
        }

        _logger.LogDebug("Successfully saved {EventCount} domain events to outbox", events.Count);
    }

    public async Task<IEnumerable<OutboxMessage>> GetUnprocessedMessagesAsync(
        int batchSize = 100,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var messages = await _outboxRepository.GetUnprocessedMessagesAsync(
                batchSize,
                cancellationToken
            );

            _logger.LogDebug(
                "Retrieved {MessageCount} unprocessed messages from outbox",
                messages.Count()
            );

            return messages;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve unprocessed messages from outbox");
            throw;
        }
    }

    public async Task MarkAsProcessedAsync(
        Guid messageId,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var message = await _outboxRepository.GetByIdAsync(messageId, cancellationToken);
            if (message == null)
            {
                _logger.LogWarning(
                    "Outbox message {MessageId} not found when marking as processed",
                    messageId
                );
                return;
            }

            message.MarkAsProcessed();
            await _outboxRepository.UpdateAsync(message, cancellationToken);

            _logger.LogDebug("Outbox message {MessageId} marked as processed", messageId);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to mark outbox message {MessageId} as processed",
                messageId
            );
            throw;
        }
    }

    public async Task MarkAsFailedAsync(
        Guid messageId,
        string errorMessage,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var message = await _outboxRepository.GetByIdAsync(messageId, cancellationToken);
            if (message == null)
            {
                _logger.LogWarning(
                    "Outbox message {MessageId} not found when marking as failed",
                    messageId
                );
                return;
            }

            message.MarkAsFailed(errorMessage);
            await _outboxRepository.UpdateAsync(message, cancellationToken);

            _logger.LogDebug(
                "Outbox message {MessageId} marked as failed with error: {Error}",
                messageId,
                errorMessage
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to mark outbox message {MessageId} as failed", messageId);
            throw;
        }
    }

    public async Task<IEnumerable<OutboxMessage>> GetFailedMessagesForRetryAsync(
        int maxRetryCount = 3,
        int batchSize = 50,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var messages = await _outboxRepository.GetFailedMessagesForRetryAsync(
                maxRetryCount,
                batchSize,
                cancellationToken
            );

            _logger.LogDebug(
                "Retrieved {MessageCount} failed messages for retry from outbox",
                messages.Count()
            );

            return messages;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve failed messages for retry from outbox");
            throw;
        }
    }

    public async Task<int> CleanupProcessedMessagesAsync(
        int olderThanDays = 7,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-olderThanDays);
            var deletedCount = await _outboxRepository.DeleteProcessedMessagesOlderThanAsync(
                cutoffDate,
                cancellationToken
            );

            _logger.LogInformation(
                "Cleaned up {DeletedCount} processed outbox messages older than {CutoffDate}",
                deletedCount,
                cutoffDate
            );

            return deletedCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cleanup processed outbox messages");
            throw;
        }
    }

    public async Task<OutboxStatistics> GetStatisticsAsync(
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var totalMessages = await _outboxRepository.GetTotalMessageCountAsync(
                cancellationToken
            );
            var processedMessages = await _outboxRepository.GetProcessedMessageCountAsync(
                cancellationToken
            );
            var failedMessages = await _outboxRepository.GetFailedMessageCountAsync(
                cancellationToken
            );
            var retryableMessages = await _outboxRepository.GetRetryableMessageCountAsync(
                cancellationToken
            );
            var oldestUnprocessed = await _outboxRepository.GetOldestUnprocessedMessageDateAsync(
                cancellationToken
            );
            var messagesByType = await _outboxRepository.GetMessageCountByEventTypeAsync(
                cancellationToken
            );

            var pendingMessages = totalMessages - processedMessages;

            var statistics = new OutboxStatistics(
                totalMessages,
                processedMessages,
                pendingMessages,
                failedMessages,
                retryableMessages,
                oldestUnprocessed,
                messagesByType
            );

            _logger.LogDebug("Retrieved outbox statistics: {Statistics}", statistics);

            return statistics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve outbox statistics");
            throw;
        }
    }

    public async Task<bool> RetryMessageAsync(
        Guid messageId,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var message = await _outboxRepository.GetByIdAsync(messageId, cancellationToken);
            if (message == null)
            {
                _logger.LogWarning("Outbox message {MessageId} not found for retry", messageId);
                return false;
            }

            if (!message.ShouldRetry)
            {
                _logger.LogWarning(
                    "Outbox message {MessageId} should not be retried (max retries exceeded)",
                    messageId
                );
                return false;
            }

            // Reset the message for retry by clearing the error and updating timestamp
            message.ResetForRetry();
            await _outboxRepository.UpdateAsync(message, cancellationToken);

            _logger.LogDebug("Outbox message {MessageId} reset for retry", messageId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retry outbox message {MessageId}", messageId);
            return false;
        }
    }

    public async Task<OutboxMessagePage> GetMessagesByEventTypeAsync(
        string eventType,
        int pageSize = 20,
        int pageIndex = 0,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var (messages, totalCount) = await _outboxRepository.GetMessagesByEventTypeAsync(
                eventType,
                pageSize,
                pageIndex,
                cancellationToken
            );

            var hasNextPage = (pageIndex + 1) * pageSize < totalCount;

            var page = new OutboxMessagePage(
                messages.ToList().AsReadOnly(),
                totalCount,
                pageIndex,
                pageSize,
                hasNextPage
            );

            _logger.LogDebug(
                "Retrieved page {PageIndex} of messages for event type {EventType}",
                pageIndex,
                eventType
            );

            return page;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to retrieve messages by event type {EventType}",
                eventType
            );
            throw;
        }
    }
}
