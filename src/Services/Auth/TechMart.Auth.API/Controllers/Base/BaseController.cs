using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TechMart.Auth.Domain.Primitives;

namespace TechMart.Auth.API.Controllers.Base;

/// <summary>
/// Base controller with common functionality for all API controllers
/// </summary>
[ApiController]
public abstract class BaseController : ControllerBase
{
    /// <summary>
    /// Gets the current user ID from JWT claims
    /// </summary>
    protected Guid GetCurrentUserId()
    {
        var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("User ID not found in token");
        }

        return userId;
    }

    /// <summary>
    /// Gets the current user email from JWT claims
    /// </summary>
    protected string GetCurrentUserEmail()
    {
        var email = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email))
        {
            throw new UnauthorizedAccessException("User email not found in token");
        }

        return email;
    }

    /// <summary>
    /// Gets user roles from JWT claims
    /// </summary>
    protected IEnumerable<string> GetCurrentUserRoles()
    {
        return HttpContext.User.FindAll(ClaimTypes.Role).Select(c => c.Value);
    }

    /// <summary>
    /// Checks if current user has a specific role
    /// </summary>
    protected bool HasRole(string role)
    {
        return HttpContext.User.IsInRole(role);
    }

    /// <summary>
    /// Gets the access token from Authorization header
    /// </summary>
    protected string GetAccessToken()
    {
        var authorizationHeader = HttpContext.Request.Headers.Authorization.FirstOrDefault();

        if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
        {
            throw new UnauthorizedAccessException("Access token not found");
        }

        return authorizationHeader["Bearer ".Length..];
    }

    /// <summary>
    /// Creates a successful API response
    /// </summary>
    protected ApiResponse<T> CreateSuccessResponse<T>(T data, string message = "Success")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            Timestamp = DateTime.UtcNow,
        };
    }

    /// <summary>
    /// Creates a successful API response without data
    /// </summary>
    protected ApiResponse CreateSuccessResponse(string message = "Success")
    {
        return new ApiResponse
        {
            Success = true,
            Message = message,
            Timestamp = DateTime.UtcNow,
        };
    }

    /// <summary>
    /// Creates an error API response
    /// </summary>
    protected ApiResponse CreateErrorResponse(Error error)
    {
        return new ApiResponse
        {
            Success = false,
            Message = error.Message,
            ErrorCode = error.Code,
            ErrorType = error.Type.ToString(),
            Timestamp = DateTime.UtcNow,
        };
    }

    /// <summary>
    /// Creates an error API response with custom message
    /// </summary>
    protected ApiResponse CreateErrorResponse(string message, string errorCode = "GENERAL_ERROR")
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            ErrorCode = errorCode,
            Timestamp = DateTime.UtcNow,
        };
    }
}

/// <summary>
/// Standard API response wrapper
/// </summary>
public class ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? ErrorCode { get; set; }
    public string? ErrorType { get; set; }
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// API response wrapper with data
/// </summary>
public class ApiResponse<T> : ApiResponse
{
    public T? Data { get; set; }
}

/// <summary>
/// Paginated API response
/// </summary>
public class PaginatedApiResponse<T> : ApiResponse<IEnumerable<T>>
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}
