using Microsoft.EntityFrameworkCore;
using TechMart.Auth.Domain.Primitives;

namespace TechMart.Auth.Infrastructure.Persistence.Repositories;

internal sealed class OutboxRepository(AuthDbContext context) : IOutboxRepository
{
    private readonly AuthDbContext _context =
        context ?? throw new ArgumentNullException(nameof(context));

    public async Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        await _context.OutboxMessages.AddAsync(message, cancellationToken);
    }

    public async Task<IEnumerable<OutboxMessage>> GetUnprocessedMessagesAsync(
        int batchSize = 100,
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .OutboxMessages.Where(m => m.ProcessedAt == null)
            .OrderBy(m => m.OccurredAt)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(
        OutboxMessage message,
        CancellationToken cancellationToken = default
    )
    {
        _context.OutboxMessages.Update(message);
    }

    public async Task DeleteAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        var message = await _context.OutboxMessages.FirstOrDefaultAsync(
            m => m.Id == messageId,
            cancellationToken
        );

        if (message != null)
        {
            _context.OutboxMessages.Remove(message);
        }
    }

    public async Task<OutboxMessage?> GetByIdAsync(
        Guid messageId,
        CancellationToken cancellationToken = default
    )
    {
        return await _context.OutboxMessages.FirstOrDefaultAsync(
            m => m.Id == messageId,
            cancellationToken
        );
    }

    public async Task<IEnumerable<OutboxMessage>> GetFailedMessagesForRetryAsync(
        int maxRetryCount = 3,
        int batchSize = 50,
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .OutboxMessages.Where(m =>
                m.ProcessedAt == null
                && !string.IsNullOrEmpty(m.Error)
                && m.RetryCount < maxRetryCount
            )
            .OrderBy(m => m.UpdatedAt)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> DeleteProcessedMessagesOlderThanAsync(
        DateTime cutoffDate,
        CancellationToken cancellationToken = default
    )
    {
        var messagesToDelete = await _context
            .OutboxMessages.Where(m => m.ProcessedAt != null && m.CreatedAt < cutoffDate)
            .ToListAsync(cancellationToken);

        if (messagesToDelete.Any())
        {
            _context.OutboxMessages.RemoveRange(messagesToDelete);
        }

        return messagesToDelete.Count;
    }

    // Métodos para estadísticas
    public async Task<int> GetTotalMessageCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.OutboxMessages.CountAsync(cancellationToken);
    }

    public async Task<int> GetProcessedMessageCountAsync(
        CancellationToken cancellationToken = default
    )
    {
        return await _context.OutboxMessages.CountAsync(
            m => m.ProcessedAt != null,
            cancellationToken
        );
    }

    public async Task<int> GetFailedMessageCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.OutboxMessages.CountAsync(
            m => m.ProcessedAt == null && !string.IsNullOrEmpty(m.Error),
            cancellationToken
        );
    }

    public async Task<int> GetRetryableMessageCountAsync(
        CancellationToken cancellationToken = default
    )
    {
        return await _context.OutboxMessages.CountAsync(
            m => m.ProcessedAt == null && !string.IsNullOrEmpty(m.Error) && m.RetryCount < 3,
            cancellationToken
        );
    }

    public async Task<DateTime?> GetOldestUnprocessedMessageDateAsync(
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .OutboxMessages.Where(m => m.ProcessedAt == null)
            .OrderBy(m => m.OccurredAt)
            .Select(m => m.OccurredAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Dictionary<string, int>> GetMessageCountByEventTypeAsync(
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .OutboxMessages.GroupBy(m => m.EventType)
            .ToDictionaryAsync(g => g.Key, g => g.Count(), cancellationToken);
    }

    public async Task<(
        IEnumerable<OutboxMessage> Messages,
        int TotalCount
    )> GetMessagesByEventTypeAsync(
        string eventType,
        int pageSize = 20,
        int pageIndex = 0,
        CancellationToken cancellationToken = default
    )
    {
        var query = _context.OutboxMessages.Where(m => m.EventType == eventType);

        var totalCount = await query.CountAsync(cancellationToken);

        var messages = await query
            .OrderByDescending(m => m.OccurredAt)
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (messages, totalCount);
    }
}
