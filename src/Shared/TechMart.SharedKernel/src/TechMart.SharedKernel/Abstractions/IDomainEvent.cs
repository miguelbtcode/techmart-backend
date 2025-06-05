using MediatR;

namespace TechMart.SharedKernel.Abstractions;

/// <summary>
/// Marker interface for domain events that occur within the domain.
/// Domain events represent something that happened in the domain that domain experts care about.
/// </summary>
public interface IDomainEvent : INotification
{
    /// <summary>
    /// Gets the date and time when the event occurred.
    /// </summary>
    DateTime OccurredOn { get; }
    
    /// <summary>
    /// Gets the unique identifier of the event.
    /// </summary>
    Guid EventId { get; }
}