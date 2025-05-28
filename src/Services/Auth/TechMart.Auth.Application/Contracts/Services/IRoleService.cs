using TechMart.Auth.Application.Common.Models;
using TechMart.Auth.Application.Features.Roles.Dtos;
using TechMart.Auth.Domain.Primitives;

namespace TechMart.Auth.Application.Contracts.Services;

/// <summary>
/// Application service for role management operations
/// </summary>
public interface IRoleService
{
    /// <summary>
    /// Gets all roles
    /// </summary>
    Task<Result<IEnumerable<RoleDto>>> GetAllRolesAsync(
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets paginated list of roles
    /// </summary>
    Task<Result<PaginatedResult<RoleDto>>> GetRolesAsync(
        PaginationParams paginationParams,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets a role by ID
    /// </summary>
    Task<Result<RoleDto>> GetRoleByIdAsync(
        Guid roleId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets a role by name
    /// </summary>
    Task<Result<RoleDto>> GetRoleByNameAsync(
        string roleName,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Assigns a role to a user
    /// </summary>
    Task<Result> AssignRoleToUserAsync(
        Guid userId,
        Guid roleId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Assigns a role to a user by role name
    /// </summary>
    Task<Result> AssignRoleToUserAsync(
        Guid userId,
        string roleName,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Removes a role from a user
    /// </summary>
    Task<Result> RemoveRoleFromUserAsync(
        Guid userId,
        Guid roleId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Removes a role from a user by role name
    /// </summary>
    Task<Result> RemoveRoleFromUserAsync(
        Guid userId,
        string roleName,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets all users with a specific role
    /// </summary>
    Task<Result<PaginatedResult<UserListDto>>> GetUsersInRoleAsync(
        Guid roleId,
        PaginationParams paginationParams,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets all users with a specific role by name
    /// </summary>
    Task<Result<PaginatedResult<UserListDto>>> GetUsersInRoleAsync(
        string roleName,
        PaginationParams paginationParams,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Checks if a user has a specific role
    /// </summary>
    Task<Result<bool>> UserHasRoleAsync(
        Guid userId,
        string roleName,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets the hierarchy level of a role
    /// </summary>
    Task<Result<int>> GetRoleHierarchyLevelAsync(
        string roleName,
        CancellationToken cancellationToken = default
    );
}
