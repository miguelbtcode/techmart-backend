using TechMart.Auth.Application.Common.Models;
using TechMart.Auth.Application.Features.Users.Dtos;
using TechMart.Auth.Domain.Primitives;
using TechMart.Auth.Domain.Users.Enums;
using TechMart.Auth.Domain.Users.ValueObjects;

namespace TechMart.Auth.Application.Contracts.Services;

/// <summary>
/// Application service for user management operations
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Creates a new user account
    /// </summary>
    Task<Result<CreateUserDto>> CreateUserAsync(
        CreateUserDto createUserDto,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets a user by ID
    /// </summary>
    Task<Result<UserDetailDto>> GetUserByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets a user by email
    /// </summary>
    Task<Result<UserDetailDto>> GetUserByEmailAsync(
        string email,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets paginated list of users
    /// </summary>
    Task<Result<PaginatedResult<UserListDto>>> GetUsersAsync(
        PaginationParams paginationParams,
        UserStatus? statusFilter = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Updates user profile information
    /// </summary>
    Task<Result> UpdateUserProfileAsync(
        Guid userId,
        UpdateUserProfileDto updateDto,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Changes user password
    /// </summary>
    Task<Result> ChangePasswordAsync(
        Guid userId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Activates a user account
    /// </summary>
    Task<Result> ActivateUserAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deactivates a user account
    /// </summary>
    Task<Result> DeactivateUserAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Suspends a user account
    /// </summary>
    Task<Result> SuspendUserAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Soft deletes a user account
    /// </summary>
    Task<Result> DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an email is available for registration
    /// </summary>
    Task<Result<EmailAvailabilityDto>> CheckEmailAvailabilityAsync(
        string email,
        Guid? excludeUserId = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Confirms user email address
    /// </summary>
    Task<Result> ConfirmEmailAsync(
        string email,
        string token,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets user roles
    /// </summary>
    Task<Result<UserRolesDto>> GetUserRolesAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    );
}
