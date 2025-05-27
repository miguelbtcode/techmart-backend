using Microsoft.EntityFrameworkCore;
using TechMart.Auth.Domain.Primitives;
using TechMart.Auth.Infrastructure.Persistence;

namespace TechMart.Auth.Infrastructure.Persistence.Repositories;

internal sealed class OutboxRepository : IOutboxRepository
{
    private readonly AuthDbContext _context;

    public OutboxRepository(AuthDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

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
        // Changes will be saved by UnitOfWork
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
}
