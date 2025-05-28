using TechMart.Auth.Application.Common.Events;

namespace TechMart.Auth.Application.Features.Authentication.Events;

public sealed record PasswordResetRequestedEvent(
    Guid UserId,
    string Email,
    string? IpAddress = null
) : IApplicationEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
    public string EventType { get; } = nameof(PasswordResetRequestedEvent);
    public string? CorrelationId { get; init; }
    Guid? IApplicationEvent.UserId => UserId;
}
