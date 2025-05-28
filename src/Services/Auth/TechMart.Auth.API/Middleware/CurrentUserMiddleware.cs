using System.Security.Claims;
using TechMart.Auth.Application.Contracts.Context;

namespace TechMart.Auth.API.Middleware;

/// <summary>
/// Middleware to populate current user context from HTTP context
/// Only lives in the API layer where HTTP dependencies are acceptable
/// </summary>
public sealed class CurrentUserMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CurrentUserMiddleware> _logger;

    public CurrentUserMiddleware(RequestDelegate next, ILogger<CurrentUserMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ICurrentUserContext userContext)
    {
        try
        {
            // Populate user context from HTTP context
            PopulateUserContext(context, userContext);

            await _next(context);
        }
        finally
        {
            // Clear context after request
            userContext.Clear();
        }
    }

    private void PopulateUserContext(HttpContext httpContext, ICurrentUserContext userContext)
    {
        var user = httpContext.User;
        if (user?.Identity?.IsAuthenticated != true)
        {
            return;
        }

        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            _logger.LogWarning("Authenticated user has invalid or missing user ID claim");
            return;
        }

        var email = user.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
        var userName = user.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
        var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList().AsReadOnly();

        var ipAddress = GetClientIpAddress(httpContext);
        var userAgent = httpContext.Request.Headers.UserAgent.FirstOrDefault();

        var userInfo = new CurrentUserInfo(userId, email, userName, roles, ipAddress, userAgent);

        userContext.SetCurrentUser(userInfo);
    }

    private static string? GetClientIpAddress(HttpContext httpContext)
    {
        // Try X-Forwarded-For first (for load balancers/proxies)
        var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            var ips = forwardedFor.Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (ips.Length > 0)
            {
                return ips[0].Trim();
            }
        }

        // Try X-Real-IP
        var realIp = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
        {
            return realIp;
        }

        // Fallback to connection remote IP
        return httpContext.Connection.RemoteIpAddress?.ToString();
    }
}
