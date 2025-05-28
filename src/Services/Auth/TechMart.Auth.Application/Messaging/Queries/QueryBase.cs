namespace TechMart.Auth.Application.Messaging.Queries;

/// <summary>
/// Base class for queries with common properties
/// </summary>
public abstract record QueryBase<TResponse> : IQuery<TResponse>
{
    /// <summary>
    /// Unique identifier for this query instance
    /// </summary>
    public Guid QueryId { get; init; } = Guid.NewGuid();

    /// <summary>
    /// When this query was created
    /// </summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// User who initiated this query (if applicable)
    /// </summary>
    public Guid? UserId { get; init; }

    /// <summary>
    /// Optional correlation ID for tracking related operations
    /// </summary>
    public string? CorrelationId { get; init; }
}
