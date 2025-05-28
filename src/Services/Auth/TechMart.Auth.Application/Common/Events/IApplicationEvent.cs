namespace TechMart.Auth.Application.Common.Events;

/// <summary>
/// Base interface for all application events
/// Application events are different from domain events - they represent application-level concerns
/// </summary>
public interface IApplicationEvent
{
    /// <summary>
    /// Unique identifier for the event instance
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// When the event occurred (UTC)
    /// </summary>
    DateTime OccurredAt { get; }

    /// <summary>
    /// Name/type of the event for identification and routing
    /// </summary>
    string EventType { get; }

    /// <summary>
    /// Optional correlation ID to link related events
    /// </summary>
    string? CorrelationId { get; }

    /// <summary>
    /// User who triggered this event (if applicable)
    /// </summary>
    Guid? UserId { get; }
}
