using TechMart.Auth.Application.Common.Events;

namespace TechMart.Auth.Application.Features.Authentication.Events;

public sealed record TokenRefreshedEvent(Guid UserId, string Email, DateTime RefreshTime = default)
    : IApplicationEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = RefreshTime == default ? DateTime.UtcNow : RefreshTime;
    public string EventType { get; } = nameof(TokenRefreshedEvent);
    public string? CorrelationId { get; init; }
    Guid? IApplicationEvent.UserId => UserId;
}
