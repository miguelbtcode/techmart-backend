using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechMart.Auth.API.Controllers.Base;
using TechMart.Auth.Application.Abstractions.Contracts;
using TechMart.Auth.Application.Abstractions.Messaging;
using TechMart.Auth.Application.Features.Users.Commands.ChangePassword;
using TechMart.Auth.Application.Features.Users.Commands.ConfirmEmail;
using TechMart.Auth.Application.Features.Users.Commands.ForgotPassword;
using TechMart.Auth.Application.Features.Users.Commands.Login;
using TechMart.Auth.Application.Features.Users.Commands.RegisterUser;
using TechMart.Auth.Application.Features.Users.Commands.ResetPassword;
using TechMart.Auth.Application.Features.Users.Queries.CheckEmailAvailability;
using TechMart.Auth.Application.Features.Users.Queries.GetCurrentUser;
using TechMart.Auth.Application.Features.Users.Queries.ValidateRefreshToken;
using TechMart.Auth.Application.Features.Users.Vms;

namespace TechMart.Auth.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : BaseController
{
    private readonly ICommandHandler<RegisterUserCommand, RegisterUserVm> _registerHandler;
    private readonly ICommandHandler<LoginCommand, LoginVm> _loginHandler;
    private readonly ICommandHandler<ConfirmEmailCommand> _confirmEmailHandler;
    private readonly ICommandHandler<ForgotPasswordCommand> _forgotPasswordHandler;
    private readonly ICommandHandler<ResetPasswordCommand> _resetPasswordHandler;
    private readonly ICommandHandler<ChangePasswordCommand> _changePasswordHandler;
    private readonly IQueryHandler<
        CheckEmailAvailabilityQuery,
        EmailAvailabilityVm
    > _checkEmailHandler;
    private readonly IQueryHandler<GetCurrentUserQuery, UserDetailVm> _getCurrentUserHandler;
    private readonly IQueryHandler<
        ValidateRefreshTokenQuery,
        TokenValidationVm
    > _validateTokenHandler;
    private readonly IRefreshTokenService _refreshTokenService;

    public AuthController(
        ICommandHandler<RegisterUserCommand, RegisterUserVm> registerHandler,
        ICommandHandler<LoginCommand, LoginVm> loginHandler,
        ICommandHandler<ConfirmEmailCommand> confirmEmailHandler,
        ICommandHandler<ForgotPasswordCommand> forgotPasswordHandler,
        ICommandHandler<ResetPasswordCommand> resetPasswordHandler,
        ICommandHandler<ChangePasswordCommand> changePasswordHandler,
        IQueryHandler<CheckEmailAvailabilityQuery, EmailAvailabilityVm> checkEmailHandler,
        IQueryHandler<GetCurrentUserQuery, UserDetailVm> getCurrentUserHandler,
        IQueryHandler<ValidateRefreshTokenQuery, TokenValidationVm> validateTokenHandler,
        IRefreshTokenService refreshTokenService
    )
    {
        _registerHandler = registerHandler;
        _loginHandler = loginHandler;
        _confirmEmailHandler = confirmEmailHandler;
        _forgotPasswordHandler = forgotPasswordHandler;
        _resetPasswordHandler = resetPasswordHandler;
        _changePasswordHandler = changePasswordHandler;
        _checkEmailHandler = checkEmailHandler;
        _getCurrentUserHandler = getCurrentUserHandler;
        _validateTokenHandler = validateTokenHandler;
        _refreshTokenService = refreshTokenService;
    }

    /// <summary>
    /// Register a new user account
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<RegisterUserVm>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        var command = new RegisterUserCommand(
            request.Email,
            request.Password,
            request.ConfirmPassword,
            request.FirstName,
            request.LastName
        );

        var result = await _registerHandler.Handle(command, HttpContext.RequestAborted);

        return result.IsSuccess
            ? CreatedAtAction(
                nameof(GetCurrentUser),
                new { },
                CreateSuccessResponse(result.Value, "User registered successfully")
            )
            : BadRequest(CreateErrorResponse(result.Error));
    }

    /// <summary>
    /// Login with email and password
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<LoginVm>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var command = new LoginCommand(request.Email, request.Password, GetClientIpAddress());
        var result = await _loginHandler.Handle(command, HttpContext.RequestAborted);

        return result.IsSuccess
            ? Ok(CreateSuccessResponse(result.Value, "Login successful"))
            : Unauthorized(CreateErrorResponse(result.Error));
    }

    /// <summary>
    /// Confirm user email address
    /// </summary>
    [HttpPost("confirm-email")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request)
    {
        var command = new ConfirmEmailCommand(request.Email, request.Token);
        var result = await _confirmEmailHandler.Handle(command, HttpContext.RequestAborted);

        return result.IsSuccess
            ? Ok(CreateSuccessResponse("Email confirmed successfully"))
            : BadRequest(CreateErrorResponse(result.Error));
    }

    /// <summary>
    /// Request password reset
    /// </summary>
    [HttpPost("forgot-password")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var command = new ForgotPasswordCommand(request.Email);
        var result = await _forgotPasswordHandler.Handle(command, HttpContext.RequestAborted);

        // Always return success for security reasons (don't reveal if email exists)
        return Ok(
            CreateSuccessResponse("If the email exists, password reset instructions have been sent")
        );
    }

    /// <summary>
    /// Reset password with token
    /// </summary>
    [HttpPost("reset-password")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var command = new ResetPasswordCommand(request.Email, request.Token, request.NewPassword);
        var result = await _resetPasswordHandler.Handle(command, HttpContext.RequestAborted);

        return result.IsSuccess
            ? Ok(CreateSuccessResponse("Password reset successfully"))
            : BadRequest(CreateErrorResponse(result.Error));
    }

    /// <summary>
    /// Change password for authenticated user
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userId = GetCurrentUserId();
        var command = new ChangePasswordCommand(
            userId,
            request.CurrentPassword,
            request.NewPassword
        );
        var result = await _changePasswordHandler.Handle(command, HttpContext.RequestAborted);

        return result.IsSuccess
            ? Ok(CreateSuccessResponse("Password changed successfully"))
            : BadRequest(CreateErrorResponse(result.Error));
    }

    /// <summary>
    /// Get current authenticated user information
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<UserDetailVm>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCurrentUser()
    {
        var accessToken = GetAccessToken();
        var query = new GetCurrentUserQuery(accessToken);
        var result = await _getCurrentUserHandler.Handle(query, HttpContext.RequestAborted);

        return result.IsSuccess
            ? Ok(CreateSuccessResponse(result.Value))
            : Unauthorized(CreateErrorResponse(result.Error));
    }

    /// <summary>
    /// Check if email is available for registration
    /// </summary>
    [HttpGet("check-email")]
    [ProducesResponseType(typeof(ApiResponse<EmailAvailabilityVm>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CheckEmailAvailability(
        [FromQuery] string email,
        [FromQuery] Guid? excludeUserId = null
    )
    {
        var query = new CheckEmailAvailabilityQuery(email, excludeUserId);
        var result = await _checkEmailHandler.Handle(query, HttpContext.RequestAborted);

        return result.IsSuccess
            ? Ok(CreateSuccessResponse(result.Value))
            : BadRequest(CreateErrorResponse(result.Error));
    }

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(ApiResponse<RefreshTokenResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var query = new ValidateRefreshTokenQuery(request.RefreshToken);
        var result = await _validateTokenHandler.Handle(query, HttpContext.RequestAborted);

        if (!result.IsSuccess || !result.Value.IsValid || !result.Value.UserId.HasValue)
        {
            return Unauthorized(CreateErrorResponse("Invalid refresh token"));
        }

        // Generate new tokens
        var newRefreshToken =
            await _refreshTokenService.GenerateRefreshTokenAsync(
            /* user object needed here */);

        var response = new RefreshTokenResponse
        {
            AccessToken = "new-access-token", // Generate new access token
            RefreshToken = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
        };

        return Ok(CreateSuccessResponse(response, "Token refreshed successfully"));
    }

    /// <summary>
    /// Logout user (revoke refresh token)
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout()
    {
        var userId = GetCurrentUserId();
        await _refreshTokenService.RevokeRefreshTokenAsync(userId);

        return Ok(CreateSuccessResponse("Logged out successfully"));
    }

    private string GetClientIpAddress()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    }
}

// Request DTOs
public record RegisterUserRequest(
    string Email,
    string Password,
    string ConfirmPassword,
    string FirstName,
    string LastName
);

public record LoginRequest(string Email, string Password);

public record ConfirmEmailRequest(string Email, string Token);

public record ForgotPasswordRequest(string Email);

public record ResetPasswordRequest(string Email, string Token, string NewPassword);

public record ChangePasswordRequest(string CurrentPassword, string NewPassword);

public record RefreshTokenRequest(string RefreshToken);

// Response DTOs
public record RefreshTokenResponse
{
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
}
