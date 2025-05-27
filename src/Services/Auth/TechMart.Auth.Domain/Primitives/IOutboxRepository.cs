namespace TechMart.Auth.Domain.Primitives;

public interface IOutboxRepository
{
    Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default);
    Task<IEnumerable<OutboxMessage>> GetUnprocessedMessagesAsync(
        int batchSize = 100,
        CancellationToken cancellationToken = default
    );
    Task UpdateAsync(OutboxMessage message, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid messageId, CancellationToken cancellationToken = default);
}
