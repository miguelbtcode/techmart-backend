using TechMart.Auth.Domain.Primitives;

namespace TechMart.Auth.Domain.Users.Events;

/// <summary>
/// Event raised when a user changes their password
/// </summary>
/// <param name="UserId">ID of the user who changed their password</param>
/// <param name="Email">Email address of the user</param>
/// <param name="WasResetRequest">Whether this was a password reset (forgot password) or normal change</param>
public sealed record UserPasswordChangedEvent(
    Guid UserId,
    string Email,
    bool WasResetRequest = false
) : DomainEventBase;
