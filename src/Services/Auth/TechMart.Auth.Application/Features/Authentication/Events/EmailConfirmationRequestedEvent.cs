using TechMart.Auth.Application.Common.Events;

namespace TechMart.Auth.Application.Features.Authentication.Events;

public sealed record EmailConfirmationRequestedEvent(Guid UserId, string Email) : IApplicationEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
    public string EventType { get; } = nameof(EmailConfirmationRequestedEvent);
    public string? CorrelationId { get; init; }
    Guid? IApplicationEvent.UserId => UserId;
}
