namespace TechMart.Auth.Application.Common.Events;

/// <summary>
/// Handler for application events
/// </summary>
/// <typeparam name="TEvent">Type of event to handle</typeparam>
public interface IEventHandler<in TEvent>
    where TEvent : IApplicationEvent
{
    /// <summary>
    /// Handle the application event
    /// </summary>
    Task Handle(TEvent @event, CancellationToken cancellationToken = default);
}
