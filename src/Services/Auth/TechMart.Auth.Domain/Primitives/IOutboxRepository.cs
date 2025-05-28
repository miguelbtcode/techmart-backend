namespace TechMart.Auth.Domain.Primitives;

public interface IOutboxRepository
{
    // ✅ Métodos básicos que ya tienes
    Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default);
    Task<IEnumerable<OutboxMessage>> GetUnprocessedMessagesAsync(
        int batchSize = 100,
        CancellationToken cancellationToken = default
    );
    Task UpdateAsync(OutboxMessage message, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid messageId, CancellationToken cancellationToken = default);

    // ✅ Métodos adicionales para funcionalidad completa

    /// <summary>
    /// Get a single outbox message by ID
    /// </summary>
    Task<OutboxMessage?> GetByIdAsync(
        Guid messageId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Get failed messages that can be retried
    /// </summary>
    Task<IEnumerable<OutboxMessage>> GetFailedMessagesForRetryAsync(
        int maxRetryCount = 3,
        int batchSize = 50,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Delete processed messages older than specified date
    /// </summary>
    Task<int> DeleteProcessedMessagesOlderThanAsync(
        DateTime cutoffDate,
        CancellationToken cancellationToken = default
    );

    // ✅ Métodos para estadísticas/monitoreo

    /// <summary>
    /// Get total count of all messages
    /// </summary>
    Task<int> GetTotalMessageCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get count of processed messages
    /// </summary>
    Task<int> GetProcessedMessageCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get count of failed messages
    /// </summary>
    Task<int> GetFailedMessageCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get count of messages that can be retried
    /// </summary>
    Task<int> GetRetryableMessageCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the date of the oldest unprocessed message
    /// </summary>
    Task<DateTime?> GetOldestUnprocessedMessageDateAsync(
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Get message count grouped by event type
    /// </summary>
    Task<Dictionary<string, int>> GetMessageCountByEventTypeAsync(
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Get messages by event type with pagination
    /// </summary>
    Task<(IEnumerable<OutboxMessage> Messages, int TotalCount)> GetMessagesByEventTypeAsync(
        string eventType,
        int pageSize = 20,
        int pageIndex = 0,
        CancellationToken cancellationToken = default
    );
}
