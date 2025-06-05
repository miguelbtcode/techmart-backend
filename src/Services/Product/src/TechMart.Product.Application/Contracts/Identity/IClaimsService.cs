using TechMart.Product.Application.Context;

namespace TechMart.Product.Application.Contracts.Identity;

public interface IClaimsService
{
    bool HasPermission(IEnumerable<string>? requiredRoles = null, Dictionary<string, string>? requiredClaims = null);
    UserContext GetUserContext();
    bool HasAnyRole(params string[] roles);
    bool HasAllRoles(params string[] roles);
    string? GetClaimValue(string claimType);
    bool HasClaim(string claimType, string? claimValue = null);
}