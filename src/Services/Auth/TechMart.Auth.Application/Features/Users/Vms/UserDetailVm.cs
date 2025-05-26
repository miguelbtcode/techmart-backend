namespace TechMart.Auth.Application.Features.Users.Vms;

public sealed record UserDetailVm(
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
