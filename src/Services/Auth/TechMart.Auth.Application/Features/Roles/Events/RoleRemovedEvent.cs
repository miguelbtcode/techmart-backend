using TechMart.Auth.Application.Common.Events;

namespace TechMart.Auth.Application.Features.Roles.Events;

public sealed record RoleRemovedEvent(
    Guid UserId,
    Guid RoleId,
    string RoleName,
    Guid? RemovedBy,
    DateTime RemovedAt
) : IApplicationEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
    public string EventType { get; } = nameof(RoleRemovedEvent);
    public string? CorrelationId { get; init; }
    Guid? IApplicationEvent.UserId => UserId;
}
