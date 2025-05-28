namespace TechMart.Auth.Application.Features.Users.Queries.GetUsers;

public sealed record GetUsersQueryVm(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string FullName,
    string Status,
    bool IsEmailConfirmed,
    DateTime? LastLoginAt,
    DateTime CreatedAt,
    IEnumerable<string> Roles
);
