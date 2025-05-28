using TechMart.Auth.Application.Common.Events;

namespace TechMart.Auth.Application.Features.Authentication.Events;

public sealed record UserLoggedInEvent(
    Guid UserId,
    string Email,
    string? IpAddress = null,
    string? UserAgent = null,
    DateTime LoginTime = default
) : IApplicationEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = LoginTime == default ? DateTime.UtcNow : LoginTime;
    public string EventType { get; } = nameof(UserLoggedInEvent);
    public string? CorrelationId { get; init; }
    Guid? IApplicationEvent.UserId => UserId;
}
