using TechMart.Auth.Domain.Primitives;

namespace TechMart.Auth.Domain.Users.Events;

/// <summary>
/// Event raised when a new user registers
/// </summary>
/// <param name="UserId">ID of the newly registered user</param>
/// <param name="Email">Email address of the new user</param>
/// <param name="FirstName">First name of the new user</param>
/// <param name="LastName">Last name of the new user</param>
public sealed record UserRegisteredEvent(
    Guid UserId,
    string Email,
    string FirstName,
    string LastName
) : DomainEventBase;
