namespace TechMart.Auth.Domain.Primitives;

public sealed class OutboxMessage : Entity<Guid>
{
    private OutboxMessage()
        : base() { }

    public OutboxMessage(IDomainEvent domainEvent, string eventType, string eventData)
        : base(Guid.NewGuid())
    {
        EventId = domainEvent.Id;
        EventType = eventType;
        EventData = eventData;
        OccurredAt = domainEvent.OccurredAt;
        ProcessedAt = null;
    }

    public Guid EventId { get; private set; }
    public string EventType { get; private set; } = string.Empty;
    public string EventData { get; private set; } = string.Empty;
    public DateTime OccurredAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public string? Error { get; private set; }
    public int RetryCount { get; private set; }

    public void MarkAsProcessed()
    {
        ProcessedAt = DateTime.UtcNow;
        Error = null;
    }

    public void MarkAsFailed(string error)
    {
        Error = error;
        RetryCount++;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsProcessed => ProcessedAt.HasValue;
    public bool HasFailed => !string.IsNullOrEmpty(Error);
    public bool ShouldRetry => RetryCount < 3 && HasFailed;
}
