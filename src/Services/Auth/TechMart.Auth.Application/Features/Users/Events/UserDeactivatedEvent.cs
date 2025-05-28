using TechMart.Auth.Application.Common.Events;

namespace TechMart.Auth.Application.Features.Users.Events;

public sealed record UserDeactivatedEvent(
    Guid UserId,
    string Email,
    Guid? DeactivatedBy = null,
    string? Reason = null
) : IApplicationEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
    public string EventType { get; } = nameof(UserDeactivatedEvent);
    public string? CorrelationId { get; init; }
    Guid? IApplicationEvent.UserId => UserId;
}
