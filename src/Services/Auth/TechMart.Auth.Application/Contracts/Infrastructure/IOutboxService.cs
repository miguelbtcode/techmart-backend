using TechMart.Auth.Domain.Primitives;

namespace TechMart.Auth.Application.Contracts.Infrastructure;

/// <summary>
/// Service for managing domain events using the Outbox pattern
/// Ensures reliable event processing and eventual consistency
/// </summary>
public interface IOutboxService
{
    /// <summary>
    /// Save a single domain event to the outbox for later processing
    /// </summary>
    /// <param name="domainEvent">The domain event to save</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the asynchronous operation</returns>
    Task SaveDomainEventAsync(
        IDomainEvent domainEvent,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Save multiple domain events to the outbox for later processing
    /// </summary>
    /// <param name="domainEvents">The domain events to save</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the asynchronous operation</returns>
    Task SaveDomainEventsAsync(
        IEnumerable<IDomainEvent> domainEvents,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Get unprocessed outbox messages for background processing
    /// </summary>
    /// <param name="batchSize">Maximum number of messages to retrieve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of unprocessed outbox messages</returns>
    Task<IEnumerable<OutboxMessage>> GetUnprocessedMessagesAsync(
        int batchSize = 100,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Mark an outbox message as successfully processed
    /// </summary>
    /// <param name="messageId">ID of the message to mark as processed</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the asynchronous operation</returns>
    Task MarkAsProcessedAsync(Guid messageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Mark an outbox message as failed with error details
    /// </summary>
    /// <param name="messageId">ID of the message that failed</param>
    /// <param name="errorMessage">Error details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the asynchronous operation</returns>
    Task MarkAsFailedAsync(
        Guid messageId,
        string errorMessage,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Get messages that failed and can be retried
    /// </summary>
    /// <param name="maxRetryCount">Maximum number of retries allowed</param>
    /// <param name="batchSize">Maximum number of messages to retrieve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of failed messages eligible for retry</returns>
    Task<IEnumerable<OutboxMessage>> GetFailedMessagesForRetryAsync(
        int maxRetryCount = 3,
        int batchSize = 50,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Delete processed messages older than specified days (cleanup)
    /// </summary>
    /// <param name="olderThanDays">Delete messages older than this many days</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of messages deleted</returns>
    Task<int> CleanupProcessedMessagesAsync(
        int olderThanDays = 7,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Get statistics about outbox messages for monitoring
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Outbox statistics</returns>
    Task<OutboxStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retry a specific failed message manually
    /// </summary>
    /// <param name="messageId">ID of the message to retry</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if retry was successful, false otherwise</returns>
    Task<bool> RetryMessageAsync(Guid messageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get outbox messages by event type for monitoring/debugging
    /// </summary>
    /// <param name="eventType">Type of event to filter by</param>
    /// <param name="pageSize">Number of messages per page</param>
    /// <param name="pageIndex">Page index (0-based)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated outbox messages</returns>
    Task<OutboxMessagePage> GetMessagesByEventTypeAsync(
        string eventType,
        int pageSize = 20,
        int pageIndex = 0,
        CancellationToken cancellationToken = default
    );
}

/// <summary>
/// Statistics about outbox messages for monitoring
/// </summary>
public sealed record OutboxStatistics(
    int TotalMessages,
    int ProcessedMessages,
    int PendingMessages,
    int FailedMessages,
    int RetryableMessages,
    DateTime? OldestUnprocessedMessage,
    Dictionary<string, int> MessagesByEventType
);

/// <summary>
/// Paginated result for outbox messages
/// </summary>
public sealed record OutboxMessagePage(
    IReadOnlyList<OutboxMessage> Messages,
    int TotalCount,
    int PageIndex,
    int PageSize,
    bool HasNextPage
)
{
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
};
