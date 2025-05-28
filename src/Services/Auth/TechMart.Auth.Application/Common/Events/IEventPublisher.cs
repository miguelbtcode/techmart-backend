namespace TechMart.Auth.Application.Common.Events;

/// <summary>
/// Publisher for application events
/// </summary>
public interface IEventPublisher
{
    /// <summary>
    /// Publish a single event
    /// </summary>
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IApplicationEvent;

    /// <summary>
    /// Publish multiple events
    /// </summary>
    Task PublishAsync(
        IEnumerable<IApplicationEvent> events,
        CancellationToken cancellationToken = default
    );
}
