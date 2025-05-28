using TechMart.Auth.Application.Features.Authentication.Dtos;

namespace TechMart.Auth.Application.Contracts.Authentication;

/// <summary>
/// Service for accessing current user information
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Gets the current user ID if authenticated
    /// </summary>
    Task<Guid?> GetCurrentUserIdAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current user information if authenticated
    /// </summary>
    Task<UserInfoDto?> GetCurrentUserAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current user's roles
    /// </summary>
    Task<IReadOnlyList<string>> GetCurrentUserRolesAsync(
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Checks if the current user is authenticated
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Gets the current user's email if authenticated
    /// </summary>
    string? CurrentUserEmail { get; }

    /// <summary>
    /// Checks if the current user has a specific role
    /// </summary>
    Task<bool> IsInRoleAsync(string role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the current user has any of the specified roles
    /// </summary>
    Task<bool> IsInAnyRoleAsync(
        IEnumerable<string> roles,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets the current user's IP address
    /// </summary>
    string? GetCurrentUserIpAddress();

    /// <summary>
    /// Gets the current user's user agent
    /// </summary>
    string? GetCurrentUserAgent();
}
