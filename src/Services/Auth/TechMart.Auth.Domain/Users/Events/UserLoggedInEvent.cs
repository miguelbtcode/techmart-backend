using TechMart.Auth.Domain.Primitives;

namespace TechMart.Auth.Domain.Users.Events;

/// <summary>
/// Event raised when a user successfully logs in
/// </summary>
public sealed record UserLoggedInEvent(Guid UserId, string Email, string? IpAddress = null)
    : DomainEventBase;
