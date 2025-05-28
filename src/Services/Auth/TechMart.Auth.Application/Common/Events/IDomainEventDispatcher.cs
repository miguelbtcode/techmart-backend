// 📁 Application/Common/Events/IDomainEventDispatcher.cs
using TechMart.Auth.Domain.Primitives;

namespace TechMart.Auth.Application.Common.Events;

/// <summary>
/// Dispatcher for domain events with hybrid strategy support
/// </summary>
public interface IDomainEventDispatcher
{
    /// <summary>
    /// Dispatch a single domain event
    /// </summary>
    Task DispatchAsync<TDomainEvent>(
        TDomainEvent domainEvent,
        CancellationToken cancellationToken = default
    )
        where TDomainEvent : IDomainEvent;

    /// <summary>
    /// Dispatch multiple domain events
    /// </summary>
    Task DispatchAsync(
        IEnumerable<IDomainEvent> domainEvents,
        CancellationToken cancellationToken = default
    );
}
