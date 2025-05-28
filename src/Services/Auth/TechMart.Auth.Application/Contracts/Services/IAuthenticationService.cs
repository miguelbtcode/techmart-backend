using TechMart.Auth.Application.Features.Authentication.Dtos;
using TechMart.Auth.Domain.Primitives;

namespace TechMart.Auth.Application.Contracts.Services;

/// <summary>
/// Application service for authentication operations
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Authenticates a user with email and password
    /// </summary>
    Task<Result<LoginDto>> LoginAsync(
        string email,
        string password,
        string? ipAddress = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Logs out a user by revoking their refresh token
    /// </summary>
    Task<Result> LogoutAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs out a user from all devices
    /// </summary>
    Task<Result> LogoutFromAllDevicesAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Refreshes an access token using a refresh token
    /// </summary>
    Task<Result<TokenRefreshDto>> RefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Validates an access token
    /// </summary>
    Task<Result<TokenValidationDto>> ValidateTokenAsync(
        string accessToken,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets current user information from token
    /// </summary>
    Task<Result<UserInfoDto>> GetCurrentUserAsync(
        string accessToken,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Initiates password reset process
    /// </summary>
    Task<Result> InitiatePasswordResetAsync(
        string email,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Completes password reset using token
    /// </summary>
    Task<Result> ResetPasswordAsync(
        string email,
        string token,
        string newPassword,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Generates email confirmation token and sends email
    /// </summary>
    Task<Result> SendEmailConfirmationAsync(
        string email,
        CancellationToken cancellationToken = default
    );
}
