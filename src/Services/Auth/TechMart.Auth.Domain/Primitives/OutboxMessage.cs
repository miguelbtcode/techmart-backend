namespace TechMart.Auth.Domain.Primitives;

public sealed class OutboxMessage : Entity<Guid>
{
    // ✅ Tu constructor existente
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

    // ✅ Tus propiedades existentes
    public Guid EventId { get; private set; }
    public string EventType { get; private set; } = string.Empty;
    public string EventData { get; private set; } = string.Empty;
    public DateTime OccurredAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public string? Error { get; private set; }
    public int RetryCount { get; private set; }

    // ✅ Tus métodos existentes
    public void MarkAsProcessed()
    {
        ProcessedAt = DateTime.UtcNow;
        Error = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsFailed(string error)
    {
        Error = error;
        RetryCount++;
        UpdatedAt = DateTime.UtcNow;
    }

    // ✅ Tus propiedades calculadas existentes
    public bool IsProcessed => ProcessedAt.HasValue;
    public bool HasFailed => !string.IsNullOrEmpty(Error);
    public bool ShouldRetry => RetryCount < 3 && HasFailed;

    // ✅ NUEVOS métodos útiles

    /// <summary>
    /// Reset the message for retry by clearing error state
    /// </summary>
    public void ResetForRetry()
    {
        Error = null;
        UpdatedAt = DateTime.UtcNow;
        // Note: RetryCount is NOT reset, it accumulates
    }

    /// <summary>
    /// Check if the message is ready for processing (considering delays)
    /// </summary>
    public bool IsReadyForProcessing(TimeSpan? processingDelay = null)
    {
        if (IsProcessed)
            return false;

        var readyTime = processingDelay.HasValue
            ? OccurredAt.Add(processingDelay.Value)
            : OccurredAt;

        return DateTime.UtcNow >= readyTime;
    }

    /// <summary>
    /// Get the next retry time based on exponential backoff
    /// </summary>
    public DateTime GetNextRetryTime()
    {
        if (!HasFailed)
            return DateTime.UtcNow;

        // Exponential backoff: 2^retry_count minutes, max 30 minutes
        var delayMinutes = Math.Min(Math.Pow(2, RetryCount), 30);
        return UpdatedAt.AddMinutes(delayMinutes);
    }

    /// <summary>
    /// Check if enough time has passed for the next retry attempt
    /// </summary>
    public bool IsReadyForRetry()
    {
        if (!HasFailed || !ShouldRetry)
            return false;

        return DateTime.UtcNow >= GetNextRetryTime();
    }

    /// <summary>
    /// Get age of the message in hours
    /// </summary>
    public double GetAgeInHours()
    {
        return (DateTime.UtcNow - OccurredAt).TotalHours;
    }

    /// <summary>
    /// Check if this is a stale message (older than specified hours)
    /// </summary>
    public bool IsStale(double maxAgeInHours = 24)
    {
        return GetAgeInHours() > maxAgeInHours;
    }
}
