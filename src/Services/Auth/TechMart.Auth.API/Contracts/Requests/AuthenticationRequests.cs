using System.ComponentModel.DataAnnotations;
using TechMart.Auth.Domain.Primitives;
using TechMart.Auth.Domain.Users.Enums;

namespace TechMart.Auth.API.Contracts.Requests;

// Authentication Requests

public record LoginRequest([Required] [EmailAddress] string Email, [Required] string Password);

public record ConfirmEmailRequest([Required] [EmailAddress] string Email, [Required] string Token);

public record ForgotPasswordRequest([Required] [EmailAddress] string Email);

public record ResetPasswordRequest(
    [Required] [EmailAddress] string Email,
    [Required] string Token,
    [Required] [MinLength(8)] string NewPassword
);

public record ChangePasswordRequest(
    [Required] string CurrentPassword,
    [Required] [MinLength(8)] string NewPassword
);

public record RefreshTokenRequest([Required] string RefreshToken);

// User Management Requests
public record UpdateProfileRequest(
    [Required] [MaxLength(100)] string FirstName,
    [Required] [MaxLength(100)] string LastName
);

public record AssignRoleRequest([Required] Guid RoleId);

// Query Parameters
public record PaginationRequest(
    int PageIndex = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    string? SortBy = null,
    UserStatus? Status = null
);

public record CheckEmailRequest([Required] [EmailAddress] string Email, Guid? ExcludeUserId = null);
