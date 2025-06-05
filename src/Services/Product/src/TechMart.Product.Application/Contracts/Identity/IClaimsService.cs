using System.Security.Claims;
using TechMart.Product.Application.Context;

namespace TechMart.Product.Application.Contracts.Identity;

/// <summary>
/// Service for working with claims and user identity information.
/// Provides helper methods for common claim operations.
/// </summary>
public interface IClaimsService
{
    /// <summary>
    /// Validates if the current user has permission to access a resource.
    /// </summary>
    /// <param name="requiredRoles">Required roles for access.</param>
    /// <param name="requiredClaims">Required claims for access.</param>
    /// <returns>True if the user has permission; otherwise, false.</returns>
    bool HasPermission(IEnumerable<string>? requiredRoles = null, Dictionary<string, string>? requiredClaims = null);

    /// <summary>
    /// Gets the current user's context information from JWT claims.
    /// </summary>
    /// <returns>User context information extracted from the current JWT token.</returns>
    UserContext GetUserContext();

    /// <summary>
    /// Validates if the current user has any of the specified roles.
    /// </summary>
    /// <param name="roles">The roles to check for.</param>
    /// <returns>True if the user has any of the specified roles.</returns>
    bool HasAnyRole(params string[] roles);

    /// <summary>
    /// Validates if the current user has all of the specified roles.
    /// </summary>
    /// <param name="roles">The roles to check for.</param>
    /// <returns>True if the user has all of the specified roles.</returns>
    bool HasAllRoles(params string[] roles);

    /// <summary>
    /// Gets a specific claim value from the current JWT token.
    /// </summary>
    /// <param name="claimType">The claim type to retrieve.</param>
    /// <returns>The claim value if found; otherwise, null.</returns>
    string? GetClaimValue(string claimType);

    /// <summary>
    /// Checks if the current user has a specific claim with an optional value.
    /// </summary>
    /// <param name="claimType">The claim type to check for.</param>
    /// <param name="claimValue">The claim value to check for (optional).</param>
    /// <returns>True if the user has the claim; otherwise, false.</returns>
    bool HasClaim(string claimType, string? claimValue = null);
}