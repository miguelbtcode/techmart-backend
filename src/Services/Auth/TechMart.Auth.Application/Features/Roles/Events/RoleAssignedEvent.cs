using TechMart.Auth.Application.Common.Events;

namespace TechMart.Auth.Application.Features.Roles.Events;

public sealed record RoleAssignedEvent(
    Guid UserId,
    Guid RoleId,
    string RoleName,
    Guid? AssignedBy,
    DateTime AssignedAt
) : IApplicationEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
    public string EventType { get; } = nameof(RoleAssignedEvent);
    public string? CorrelationId { get; init; }
    Guid? IApplicationEvent.UserId => UserId;
}
