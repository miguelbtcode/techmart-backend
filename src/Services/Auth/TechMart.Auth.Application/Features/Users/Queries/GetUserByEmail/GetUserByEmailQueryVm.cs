namespace TechMart.Auth.Application.Features.Users.Queries.GetUserByEmail;

public sealed record GetUserByEmailQueryVm(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string Status,
    bool IsEmailConfirmed,
    DateTime? LastLoginAt,
    DateTime CreatedAt,
    IEnumerable<string> Roles
);
