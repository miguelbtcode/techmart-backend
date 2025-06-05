namespace TechMart.Product.Application.Contracts.Identity;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? UserName { get; }
    string? Email { get; }
    IEnumerable<string> Roles { get; }
    bool IsAuthenticated { get; }
    bool IsInRole(string role);
    string? GetClaimValue(string claimType);
}