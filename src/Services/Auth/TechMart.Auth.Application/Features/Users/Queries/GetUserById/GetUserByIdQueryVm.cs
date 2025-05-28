namespace TechMart.Auth.Application.Features.Users.Queries.GetUserById;

public sealed record GetUserByIdQueryVm(
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
