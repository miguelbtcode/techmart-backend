using TechMart.Auth.Application.Common.Events;

namespace TechMart.Auth.Application.Features.Authentication.Events;

public sealed record UserLoggedOutEvent(
    Guid UserId,
    string Email,
    string LogoutReason = "Manual",
    DateTime LogoutTime = default
) : IApplicationEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = LogoutTime == default ? DateTime.UtcNow : LogoutTime;
    public string EventType { get; } = nameof(UserLoggedOutEvent);
    public string? CorrelationId { get; init; }
    Guid? IApplicationEvent.UserId => UserId;
}
