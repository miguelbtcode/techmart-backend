using TechMart.Auth.Domain.Primitives;
using TechMart.Auth.Domain.Users.Enums;

namespace TechMart.Auth.Domain.Users.Events;

/// <summary>
/// Event raised when a user's status changes
/// </summary>
/// <param name="UserId">ID of the user whose status changed</param>
/// <param name="Email">Email address of the user</param>
/// <param name="PreviousStatus">Previous status</param>
/// <param name="NewStatus">New status</param>
/// <param name="ChangedBy">ID of the user who made the change (optional)</param>
public sealed record UserStatusChangedEvent(
    Guid UserId,
    string Email,
    UserStatus PreviousStatus,
    UserStatus NewStatus,
    Guid? ChangedBy = null
) : DomainEventBase;
