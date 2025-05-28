using TechMart.Auth.Application.Common.Events;
using TechMart.Auth.Domain.Users.Enums;

namespace TechMart.Auth.Application.Features.Users.Events;

public sealed record UserStatusChangedEvent(
    Guid UserId,
    string Email,
    UserStatus PreviousStatus,
    UserStatus NewStatus,
    Guid? ChangedBy = null,
    string? Reason = null
) : IApplicationEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
    public string EventType { get; } = nameof(UserStatusChangedEvent);
    public string? CorrelationId { get; init; }
    Guid? IApplicationEvent.UserId => UserId;
}
