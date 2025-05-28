using TechMart.Auth.Application.Features.Shared.Dtos;

namespace TechMart.Auth.Application.Contracts.Authentication;

/// <summary>
/// Service for accessing detailed current user information from database
/// Uses ICurrentUserContext for basic info and enriches with database data
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Gets detailed current user information from database
    /// </summary>
    Task<UserInfoVm?> GetCurrentUserAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets current user ID from context
    /// </summary>
    Task<Guid?> GetCurrentUserIdAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets current user roles from context with database fallback
    /// </summary>
    Task<IReadOnlyList<string>> GetCurrentUserRolesAsync(
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Checks if current user has a specific role
    /// </summary>
    Task<bool> IsInRoleAsync(string role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if current user has any of the specified roles
    /// </summary>
    Task<bool> IsInAnyRoleAsync(
        IEnumerable<string> roles,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Whether the current user is authenticated
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Gets current user email from context
    /// </summary>
    string? CurrentUserEmail { get; }

    /// <summary>
    /// Gets current user IP address from context
    /// </summary>
    string? GetCurrentUserIpAddress();

    /// <summary>
    /// Gets current user agent from context
    /// </summary>
    string? GetCurrentUserAgent();
}
