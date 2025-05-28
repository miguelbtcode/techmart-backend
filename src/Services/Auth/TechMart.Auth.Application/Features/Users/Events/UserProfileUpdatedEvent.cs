using TechMart.Auth.Application.Common.Events;

namespace TechMart.Auth.Application.Features.Users.Events;

public sealed record UserProfileUpdatedEvent(
    Guid UserId,
    string Email,
    string PreviousFirstName,
    string PreviousLastName,
    string NewFirstName,
    string NewLastName,
    Guid? UpdatedBy = null
) : IApplicationEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
    public string EventType { get; } = nameof(UserProfileUpdatedEvent);
    public string? CorrelationId { get; init; }
    Guid? IApplicationEvent.UserId => UserId;
}
