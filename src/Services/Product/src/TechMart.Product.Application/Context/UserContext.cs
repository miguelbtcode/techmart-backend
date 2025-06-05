namespace TechMart.Product.Application.Context;

/// <summary>
/// Represents the context information for the current user.
/// </summary>
public class UserContext
{
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? FullName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public List<string> Roles { get; set; } = new();
    public string? TenantId { get; set; }
    public string? OrganizationId { get; set; }
    public bool IsAuthenticated { get; set; }
    public Dictionary<string, string> CustomProperties { get; set; } = new();

    /// <summary>
    /// Gets an anonymous user context for unauthenticated users.
    /// </summary>
    public static UserContext Anonymous => new()
    {
        IsAuthenticated = false,
        Roles = new List<string>(),
        CustomProperties = new Dictionary<string, string>()
    };

    /// <summary>
    /// Checks if the user has any of the specified roles.
    /// </summary>
    /// <param name="roles">The roles to check.</param>
    /// <returns>True if the user has any of the specified roles.</returns>
    public bool HasAnyRole(params string[] roles)
    {
        return roles.Any(role => Roles.Contains(role, StringComparer.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Checks if the user has all of the specified roles.
    /// </summary>
    /// <param name="roles">The roles to check.</param>
    /// <returns>True if the user has all of the specified roles.</returns>
    public bool HasAllRoles(params string[] roles)
    {
        return roles.All(role => Roles.Contains(role, StringComparer.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Gets a custom property value from JWT claims.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>The property value if found; otherwise, null.</returns>
    public string? GetCustomProperty(string key)
    {
        return CustomProperties.TryGetValue(key, out var value) ? value : null;
    }

    /// <summary>
    /// Checks if the user belongs to a specific tenant (for multi-tenant scenarios).
    /// </summary>
    /// <param name="tenantId">The tenant ID to check.</param>
    /// <returns>True if the user belongs to the specified tenant.</returns>
    public bool BelongsToTenant(string tenantId)
    {
        return !string.IsNullOrWhiteSpace(TenantId) && 
               TenantId.Equals(tenantId, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if the user belongs to a specific organization.
    /// </summary>
    /// <param name="organizationId">The organization ID to check.</param>
    /// <returns>True if the user belongs to the specified organization.</returns>
    public bool BelongsToOrganization(string organizationId)
    {
        return !string.IsNullOrWhiteSpace(OrganizationId) && 
               OrganizationId.Equals(organizationId, StringComparison.OrdinalIgnoreCase);
    }
}