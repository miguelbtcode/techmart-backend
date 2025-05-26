namespace TechMart.Auth.Application.Features.Users.Vms;

public sealed record UserInfoVm(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    IEnumerable<string> Roles
);
