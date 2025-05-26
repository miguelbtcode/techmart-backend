using TechMart.Auth.Domain.Primitives;

namespace TechMart.Auth.Domain.Users.Events;

/// <summary>
/// Event raised when a user confirms their email address
/// </summary>
public sealed record UserEmailConfirmedEvent(Guid UserId, string Email) : DomainEventBase;
