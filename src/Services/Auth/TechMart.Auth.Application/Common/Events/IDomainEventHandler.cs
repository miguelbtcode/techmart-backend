// Application/Common/Events/IDomainEventHandler.cs
using TechMart.Auth.Domain.Primitives;

namespace TechMart.Auth.Application.Common.Events;

/// <summary>
/// Handler for domain events in the application layer
/// Handles business logic side effects when domain events occur
/// </summary>
/// <typeparam name="TDomainEvent">Type of domain event to handle</typeparam>
public interface IDomainEventHandler<in TDomainEvent>
    where TDomainEvent : IDomainEvent
{
    /// <summary>
    /// Handle the domain event with application-specific logic
    /// </summary>
    /// <param name="domainEvent">The domain event that occurred</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the asynchronous operation</returns>
    Task Handle(TDomainEvent domainEvent, CancellationToken cancellationToken = default);
}
