using TechMart.Product.Application.Context;
using TechMart.Product.Application.Contracts.Identity;
using TechMart.SharedKernel.Abstractions;

namespace TechMart.Product.Infrastructure.Identity;

public class CurrentUserService : ICurrentUserService
{
    private readonly IClaimsService _claimsService;
    private UserContext? _cachedUserContext;

    public CurrentUserService(IClaimsService claimsService)
    {
        _claimsService = claimsService ?? throw new ArgumentNullException(nameof(claimsService));
    }

    private UserContext UserContext => _cachedUserContext ??= _claimsService.GetUserContext();

    public string? UserId => UserContext.UserId;

    public string? UserName => UserContext.UserName;

    public string? Email => UserContext.Email;

    public IEnumerable<string> Roles => UserContext.Roles;

    public bool IsAuthenticated => UserContext.IsAuthenticated;

    public bool IsInRole(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
            return false;

        return UserContext.HasAnyRole(role);
    }

    public string? GetClaimValue(string claimType)
    {
        if (string.IsNullOrWhiteSpace(claimType))
            return null;

        // Handle standard claim mappings
        return claimType switch
        {
            "sub" or "user_id" => UserContext.UserId,
            "username" or "preferred_username" => UserContext.UserName,
            "email" => UserContext.Email,
            "name" => UserContext.FullName,
            "given_name" => UserContext.FirstName,
            "family_name" => UserContext.LastName,
            "tenant_id" => UserContext.TenantId,
            "organization_id" => UserContext.OrganizationId,
            _ => UserContext.GetCustomProperty(claimType)
        };
    }
}