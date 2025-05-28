using TechMart.Auth.Application.Common.Events;

namespace TechMart.Auth.Application.Features.Users.Events;

public sealed record UserEmailConfirmedEvent(
    Guid UserId,
    string Email,
    DateTime ConfirmedAt = default
) : IApplicationEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = ConfirmedAt == default ? DateTime.UtcNow : ConfirmedAt;
    public string EventType { get; } = nameof(UserEmailConfirmedEvent);
    public string? CorrelationId { get; init; }
    Guid? IApplicationEvent.UserId => UserId;
}
