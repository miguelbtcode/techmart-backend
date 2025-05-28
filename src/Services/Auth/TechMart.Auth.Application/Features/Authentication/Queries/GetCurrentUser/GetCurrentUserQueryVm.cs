namespace TechMart.Auth.Application.Features.Authentication.Queries.GetCurrentUser;

public sealed record GetCurrentUserQueryVm(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string Status,
    bool IsEmailConfirmed,
    DateTime? LastLoginAt,
    DateTime CreatedAt,
    IEnumerable<string> Roles
)
{
    public string FullName => $"{FirstName} {LastName}".Trim();
    public string DisplayName => string.IsNullOrWhiteSpace(FirstName) ? Email : FirstName;
}
