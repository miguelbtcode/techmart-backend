using System.Text.Json;
using Microsoft.Extensions.Options;
using TechMart.Auth.API.Common.Responses;
using TechMart.Auth.Application.Contracts.Infrastructure;

namespace TechMart.Auth.API.Middleware;

/// <summary>
/// Middleware for rate limiting API requests
/// Provides flexible rate limiting based on different identifiers
/// </summary>
public sealed class RateLimitMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IRateLimitService _rateLimitService;
    private readonly ILogger<RateLimitMiddleware> _logger;
    private readonly RateLimitOptions _options;

    public RateLimitMiddleware(
        RequestDelegate next,
        IRateLimitService rateLimitService,
        IOptions<RateLimitOptions> options,
        ILogger<RateLimitMiddleware> logger
    )
    {
        _next = next;
        _rateLimitService = rateLimitService;
        _options = options.Value;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip rate limiting for certain paths
        if (ShouldSkipRateLimit(context))
        {
            await _next(context);
            return;
        }

        var rateLimitRule = GetRateLimitRule(context);
        if (rateLimitRule == null)
        {
            await _next(context);
            return;
        }

        var identifier = GetIdentifier(context, rateLimitRule.IdentifierType);
        if (string.IsNullOrEmpty(identifier))
        {
            await _next(context);
            return;
        }

        try
        {
            // Check if request is allowed
            var isAllowed = await _rateLimitService.IsAllowedAsync(
                identifier,
                rateLimitRule.MaxAttempts,
                rateLimitRule.Window
            );

            if (!isAllowed)
            {
                await HandleRateLimitExceeded(context, identifier, rateLimitRule);
                return;
            }

            // Process the request
            var originalStatusCode = context.Response.StatusCode;
            await _next(context);

            // Only increment on failed requests (4xx, 5xx) for login attempts
            // Or always increment for general API rate limiting
            if (ShouldIncrementCounter(context, rateLimitRule, originalStatusCode))
            {
                await _rateLimitService.IncrementAsync(identifier, rateLimitRule.Window);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Rate limiting failed for identifier {Identifier}", identifier);
            // Continue processing - don't let rate limiting break the application
            await _next(context);
        }
    }

    private bool ShouldSkipRateLimit(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLowerInvariant();

        return _options.ExcludedPaths.Any(excludedPath =>
            path?.StartsWith(excludedPath.ToLowerInvariant()) == true
        );
    }

    private RateLimitRule? GetRateLimitRule(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLowerInvariant();
        var method = context.Request.Method.ToUpperInvariant();

        return _options.Rules.FirstOrDefault(rule =>
            rule.Paths.Any(rulePath => path?.StartsWith(rulePath.ToLowerInvariant()) == true)
            && (rule.Methods.Contains("*") || rule.Methods.Contains(method))
        );
    }

    private string? GetIdentifier(HttpContext context, RateLimitIdentifierType identifierType)
    {
        return identifierType switch
        {
            RateLimitIdentifierType.IpAddress => GetClientIpAddress(context),
            RateLimitIdentifierType.User => GetUserId(context),
            RateLimitIdentifierType.Email => GetEmailFromRequest(context),
            RateLimitIdentifierType.Combined => GetCombinedIdentifier(context),
            _ => null,
        };
    }

    private string? GetClientIpAddress(HttpContext context)
    {
        // Try X-Forwarded-For first (for load balancers/proxies)
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            var ips = forwardedFor.Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (ips.Length > 0)
            {
                return ips[0].Trim();
            }
        }

        return context.Connection.RemoteIpAddress?.ToString();
    }

    private string? GetUserId(HttpContext context)
    {
        return context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    }

    private string? GetEmailFromRequest(HttpContext context)
    {
        // Try to get email from claims first
        var emailFromClaims = context
            .User?.FindFirst(System.Security.Claims.ClaimTypes.Email)
            ?.Value;
        if (!string.IsNullOrEmpty(emailFromClaims))
        {
            return emailFromClaims;
        }

        // For login requests, try to extract from request body
        return ExtractEmailFromRequestBody(context);
    }

    private string? ExtractEmailFromRequestBody(HttpContext context)
    {
        try
        {
            // This is tricky with minimal APIs - would need to peek at the request body
            // For now, return null and handle in the specific endpoint
            return null;
        }
        catch
        {
            return null;
        }
    }

    private string GetCombinedIdentifier(HttpContext context)
    {
        var ip = GetClientIpAddress(context);
        var userId = GetUserId(context);
        var email = GetEmailFromRequest(context);

        // Combine available identifiers
        var identifiers = new[] { ip, userId, email }.Where(x => !string.IsNullOrEmpty(x));
        return string.Join(":", identifiers);
    }

    private bool ShouldIncrementCounter(
        HttpContext context,
        RateLimitRule rule,
        int originalStatusCode
    )
    {
        return rule.IncrementPolicy switch
        {
            RateLimitIncrementPolicy.Always => true,
            RateLimitIncrementPolicy.OnError => context.Response.StatusCode >= 400,
            RateLimitIncrementPolicy.OnClientError => context.Response.StatusCode >= 400
                && context.Response.StatusCode < 500,
            RateLimitIncrementPolicy.OnUnauthorized => context.Response.StatusCode == 401,
            _ => true,
        };
    }

    private async Task HandleRateLimitExceeded(
        HttpContext context,
        string identifier,
        RateLimitRule rule
    )
    {
        _logger.LogWarning(
            "Rate limit exceeded for {Identifier} on {Path} - Rule: {RuleName}",
            identifier,
            context.Request.Path,
            rule.Name
        );

        context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.Response.ContentType = "application/json";

        // Add rate limit headers
        context.Response.Headers.Append("X-RateLimit-Limit", rule.MaxAttempts.ToString());
        context.Response.Headers.Append("X-RateLimit-Remaining", "0");
        context.Response.Headers.Append(
            "X-RateLimit-Reset",
            DateTimeOffset.UtcNow.Add(rule.Window).ToUnixTimeSeconds().ToString()
        );

        var errorResponse = ApiResponse
            .Failure(
                "Rate limit exceeded. Please try again later.",
                new ApiError("RATE_LIMIT_EXCEEDED", rule.Message, "RateLimit")
            )
            .WithTraceId(context.TraceIdentifier);

        var json = JsonSerializer.Serialize(
            errorResponse,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
        );

        await context.Response.WriteAsync(json);
    }
}

/// <summary>
/// Configuration options for rate limiting
/// </summary>
public sealed class RateLimitOptions
{
    public const string SectionName = "RateLimit";

    public List<RateLimitRule> Rules { get; set; } = new();
    public List<string> ExcludedPaths { get; set; } = new() { "/health", "/swagger" };
    public bool EnableGlobalRateLimit { get; set; } = true;
    public TimeSpan DefaultWindow { get; set; } = TimeSpan.FromMinutes(15);
    public int DefaultMaxAttempts { get; set; } = 100; // For general API usage
}

/// <summary>
/// Rate limiting rule configuration
/// </summary>
public sealed class RateLimitRule
{
    public string Name { get; set; } = string.Empty;
    public List<string> Paths { get; set; } = new();
    public List<string> Methods { get; set; } = new() { "*" };
    public RateLimitIdentifierType IdentifierType { get; set; } = RateLimitIdentifierType.IpAddress;
    public int MaxAttempts { get; set; } = 5;
    public TimeSpan Window { get; set; } = TimeSpan.FromMinutes(15);
    public RateLimitIncrementPolicy IncrementPolicy { get; set; } =
        RateLimitIncrementPolicy.OnError;
    public string Message { get; set; } = "Too many requests. Please try again later.";
    public bool Enabled { get; set; } = true;
}

/// <summary>
/// Types of identifiers for rate limiting
/// </summary>
public enum RateLimitIdentifierType
{
    IpAddress,
    User,
    Email,
    Combined,
}

/// <summary>
/// When to increment the rate limit counter
/// </summary>
public enum RateLimitIncrementPolicy
{
    Always, // Every request
    OnError, // 4xx and 5xx responses
    OnClientError, // 4xx responses only
    OnUnauthorized, // 401 responses only
}
