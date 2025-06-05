namespace TechMart.SharedKernel.Abstractions;

/// <summary>
/// Service for accessing information about the current user.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Gets the unique identifier of the current user.
    /// </summary>
    string? UserId { get; }
    
    /// <summary>
    /// Gets the username of the current user.
    /// </summary>
    string? UserName { get; }
    
    /// <summary>
    /// Gets the email of the current user.
    /// </summary>
    string? Email { get; }
    
    /// <summary>
    /// Gets the roles of the current user.
    /// </summary>
    IEnumerable<string> Roles { get; }
    
    /// <summary>
    /// Determines whether the current user is authenticated.
    /// </summary>
    bool IsAuthenticated { get; }
    
    /// <summary>
    /// Determines whether the current user has the specified role.
    /// </summary>
    /// <param name="role">The role to check.</param>
    /// <returns>True if the user has the role; otherwise, false.</returns>
    bool IsInRole(string role);
    
    /// <summary>
    /// Gets a claim value for the current user.
    /// </summary>
    /// <param name="claimType">The type of claim to retrieve.</param>
    /// <returns>The claim value if found; otherwise, null.</returns>
    string? GetClaimValue(string claimType);
}