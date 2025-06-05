using System.Security.Claims;

namespace TechMart.SharedKernel.Extensions.Security;

/// <summary>
/// Extension methods for ClaimsPrincipal.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Gets the user ID from claims.
    /// </summary>
    /// <param name="principal">The claims principal.</param>
    /// <returns>The user ID.</returns>
    public static string? GetUserId(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
               principal.FindFirst("sub")?.Value ??
               principal.FindFirst("user_id")?.Value;
    }

    /// <summary>
    /// Gets the username from claims.
    /// </summary>
    /// <param name="principal">The claims principal.</param>
    /// <returns>The username.</returns>
    public static string? GetUserName(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimTypes.Name)?.Value ??
               principal.FindFirst("username")?.Value ??
               principal.FindFirst("preferred_username")?.Value;
    }

    /// <summary>
    /// Gets the email from claims.
    /// </summary>
    /// <param name="principal">The claims principal.</param>
    /// <returns>The email.</returns>
    public static string? GetEmail(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimTypes.Email)?.Value ??
               principal.FindFirst("email")?.Value;
    }

    /// <summary>
    /// Gets the full name from claims.
    /// </summary>
    /// <param name="principal">The claims principal.</param>
    /// <returns>The full name.</returns>
    public static string? GetFullName(this ClaimsPrincipal principal)
    {
        return principal.FindFirst("name")?.Value ??
               principal.FindFirst(ClaimTypes.GivenName)?.Value;
    }

    /// <summary>
    /// Gets the first name from claims.
    /// </summary>
    /// <param name="principal">The claims principal.</param>
    /// <returns>The first name.</returns>
    public static string? GetFirstName(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimTypes.GivenName)?.Value ??
               principal.FindFirst("given_name")?.Value ??
               principal.FindFirst("first_name")?.Value;
    }

    /// <summary>
    /// Gets the last name from claims.
    /// </summary>
    /// <param name="principal">The claims principal.</param>
    /// <returns>The last name.</returns>
    public static string? GetLastName(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimTypes.Surname)?.Value ??
               principal.FindFirst("family_name")?.Value ??
               principal.FindFirst("last_name")?.Value;
    }

    /// <summary>
    /// Gets all roles from claims.
    /// </summary>
    /// <param name="principal">The claims principal.</param>
    /// <returns>The roles.</returns>
    public static IEnumerable<string> GetRoles(this ClaimsPrincipal principal)
    {
        return principal.FindAll(ClaimTypes.Role).Select(c => c.Value);
    }

    /// <summary>
    /// Checks if the user has the specified role.
    /// </summary>
    /// <param name="principal">The claims principal.</param>
    /// <param name="role">The role to check.</param>
    /// <returns>True if user has role; otherwise, false.</returns>
    public static bool HasRole(this ClaimsPrincipal principal, string role)
    {
        return principal.IsInRole(role);
    }

    /// <summary>
    /// Checks if the user has any of the specified roles.
    /// </summary>
    /// <param name="principal">The claims principal.</param>
    /// <param name="roles">The roles to check.</param>
    /// <returns>True if user has any role; otherwise, false.</returns>
    public static bool HasAnyRole(this ClaimsPrincipal principal, params string[] roles)
    {
        return roles.Any(role => principal.IsInRole(role));
    }

    /// <summary>
    /// Checks if the user has all of the specified roles.
    /// </summary>
    /// <param name="principal">The claims principal.</param>
    /// <param name="roles">The roles to check.</param>
    /// <returns>True if user has all roles; otherwise, false.</returns>
    public static bool HasAllRoles(this ClaimsPrincipal principal, params string[] roles)
    {
        return roles.All(role => principal.IsInRole(role));
    }

    /// <summary>
    /// Gets a claim value by type.
    /// </summary>
    /// <param name="principal">The claims principal.</param>
    /// <param name="claimType">The claim type.</param>
    /// <returns>The claim value.</returns>
    public static string? GetClaimValue(this ClaimsPrincipal principal, string claimType)
    {
        return principal.FindFirst(claimType)?.Value;
    }

    /// <summary>
    /// Gets all claim values by type.
    /// </summary>
    /// <param name="principal">The claims principal.</param>
    /// <param name="claimType">The claim type.</param>
    /// <returns>The claim values.</returns>
    public static IEnumerable<string> GetClaimValues(this ClaimsPrincipal principal, string claimType)
    {
        return principal.FindAll(claimType).Select(c => c.Value);
    }

    /// <summary>
    /// Checks if the user has a specific claim.
    /// </summary>
    /// <param name="principal">The claims principal.</param>
    /// <param name="claimType">The claim type.</param>
    /// <param name="claimValue">The claim value (optional).</param>
    /// <returns>True if user has claim; otherwise, false.</returns>
    public static bool HasClaim(this ClaimsPrincipal principal, string claimType, string? claimValue = null)
    {
        if (claimValue == null)
            return claimValue != null && principal.HasClaim(claimType, claimValue);
        
        return principal.FindFirst(claimType) != null;
    }

    /// <summary>
    /// Gets the tenant ID from claims (for multi-tenant applications).
    /// </summary>
    /// <param name="principal">The claims principal.</param>
    /// <returns>The tenant ID.</returns>
    public static string? GetTenantId(this ClaimsPrincipal principal)
    {
        return principal.FindFirst("tenant_id")?.Value ??
               principal.FindFirst("tid")?.Value;
    }

    /// <summary>
    /// Gets the organization ID from claims.
    /// </summary>
    /// <param name="principal">The claims principal.</param>
    /// <returns>The organization ID.</returns>
    public static string? GetOrganizationId(this ClaimsPrincipal principal)
    {
        return principal.FindFirst("organization_id")?.Value ??
               principal.FindFirst("org_id")?.Value;
    }

    /// <summary>
    /// Checks if the user is authenticated.
    /// </summary>
    /// <param name="principal">The claims principal.</param>
    /// <returns>True if authenticated; otherwise, false.</returns>
    public static bool IsAuthenticated(this ClaimsPrincipal principal)
    {
        return principal.Identity?.IsAuthenticated == true;
    }

    /// <summary>
    /// Gets custom properties as a dictionary.
    /// </summary>
    /// <param name="principal">The claims principal.</param>
    /// <param name="prefix">The claim prefix to filter by.</param>
    /// <returns>A dictionary of custom properties.</returns>
    public static Dictionary<string, string> GetCustomProperties(this ClaimsPrincipal principal, string prefix = "custom_")
    {
        return principal.Claims
            .Where(c => c.Type.StartsWith(prefix))
            .ToDictionary(c => c.Type.Substring(prefix.Length), c => c.Value);
    }
}