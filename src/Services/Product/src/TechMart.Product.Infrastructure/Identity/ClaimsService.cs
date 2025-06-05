using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TechMart.Product.Application.Context;
using TechMart.Product.Application.Contracts.Identity;
using TechMart.SharedKernel.Extensions.Security;

namespace TechMart.Product.Infrastructure.Identity;

public class ClaimsService : IClaimsService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ClaimsService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    private ClaimsPrincipal? CurrentUser => _httpContextAccessor.HttpContext?.User;

    public bool HasPermission(IEnumerable<string>? requiredRoles = null, Dictionary<string, string>? requiredClaims = null)
    {
        var user = CurrentUser;
        if (user == null || !user.IsAuthenticated())
            return false;

        // Check required roles using SharedKernel extensions
        if (requiredRoles != null && requiredRoles.Any())
        {
            if (!user.HasAnyRole(requiredRoles.ToArray()))
                return false;
        }

        // Check required claims using SharedKernel extensions
        if (requiredClaims != null && requiredClaims.Any())
        {
            foreach (var requiredClaim in requiredClaims)
            {
                if (!user.HasClaim(requiredClaim.Key, requiredClaim.Value))
                    return false;
            }
        }

        return true;
    }

    public UserContext GetUserContext()
    {
        var user = CurrentUser;
        if (user == null || !user.IsAuthenticated())
            return UserContext.Anonymous;

        // Use SharedKernel extensions to extract user information from JWT
        return new UserContext
        {
            UserId = user.GetUserId(),
            UserName = user.GetUserName(),
            Email = user.GetEmail(),
            FullName = user.GetFullName(),
            FirstName = user.GetFirstName(),
            LastName = user.GetLastName(),
            Roles = user.GetRoles().ToList(),
            TenantId = user.GetTenantId(),
            OrganizationId = user.GetOrganizationId(),
            IsAuthenticated = user.IsAuthenticated(),
            CustomProperties = user.GetCustomProperties()
        };
    }

    public bool HasAnyRole(params string[] roles)
    {
        var user = CurrentUser;
        return user?.HasAnyRole(roles) ?? false;
    }

    public bool HasAllRoles(params string[] roles)
    {
        var user = CurrentUser;
        return user?.HasAllRoles(roles) ?? false;
    }

    public string? GetClaimValue(string claimType)
    {
        var user = CurrentUser;
        return user?.GetClaimValue(claimType);
    }

    public bool HasClaim(string claimType, string? claimValue = null)
    {
        var user = CurrentUser;
        return user?.HasClaim(claimType, claimValue) ?? false;
    }
}