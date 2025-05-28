namespace TechMart.Auth.Application.Contracts.Context;

/// <summary>
/// Context for accessing current user information without HTTP dependencies
/// This abstraction allows infrastructure to be web-agnostic
/// </summary>
public interface ICurrentUserContext
{
    /// <summary>
    /// Gets the current user ID if authenticated
    /// </summary>
    Guid? UserId { get; }

    /// <summary>
    /// Gets the current user email if authenticated
    /// </summary>
    string? UserEmail { get; }

    /// <summary>
    /// Gets the current user's full name
    /// </summary>
    string? UserName { get; }

    /// <summary>
    /// Gets the current user's roles
    /// </summary>
    IReadOnlyList<string> UserRoles { get; }

    /// <summary>
    /// Whether the current user is authenticated
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Gets the current user's IP address
    /// </summary>
    string? IpAddress { get; }

    /// <summary>
    /// Gets the current user's user agent
    /// </summary>
    string? UserAgent { get; }

    /// <summary>
    /// Sets the current user context (used by infrastructure)
    /// </summary>
    void SetCurrentUser(CurrentUserInfo userInfo);

    /// <summary>
    /// Clears the current user context
    /// </summary>
    void Clear();
}

/// <summary>
/// Data transfer object for current user information
/// </summary>
public sealed record CurrentUserInfo(
    Guid UserId,
    string Email,
    string UserName,
    IReadOnlyList<string> Roles,
    string? IpAddress = null,
    string? UserAgent = null
);
