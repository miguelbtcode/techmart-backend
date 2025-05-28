using TechMart.Auth.Application.Common.Events;

namespace TechMart.Auth.Application.Features.Users.Events;

public sealed record UserDeletedEvent(
    Guid UserId,
    string Email,
    Guid? DeletedBy = null,
    string? Reason = null
) : IApplicationEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
    public string EventType { get; } = nameof(UserDeletedEvent);
    public string? CorrelationId { get; init; }
    Guid? IApplicationEvent.UserId => UserId;
}
