using TechMart.Auth.Application.Common.Events;

namespace TechMart.Auth.Application.Features.Users.Events;

public sealed record UserCreatedEvent(
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    DateTime CreatedAt = default
) : IApplicationEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = CreatedAt == default ? DateTime.UtcNow : CreatedAt;
    public string EventType { get; } = nameof(UserCreatedEvent);
    public string? CorrelationId { get; init; }
    Guid? IApplicationEvent.UserId => UserId;
}
