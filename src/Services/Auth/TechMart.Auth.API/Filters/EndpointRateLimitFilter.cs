using System.Security.Claims;
using TechMart.Auth.API.Attributes;
using TechMart.Auth.API.Common.Responses;
using TechMart.Auth.Application.Contracts.Infrastructure;

namespace TechMart.Auth.API.Filters;

/// <summary>
/// Filter to apply rate limiting based on endpoint attributes
/// </summary>
public class EndpointRateLimitFilter : IEndpointFilter
{
    private readonly IRateLimitService _rateLimitService;
    private readonly ILogger<EndpointRateLimitFilter> _logger;

    public EndpointRateLimitFilter(
        IRateLimitService rateLimitService,
        ILogger<EndpointRateLimitFilter> logger
    )
    {
        _rateLimitService = rateLimitService;
        _logger = logger;
    }

    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next
    )
    {
        var endpoint = context.HttpContext.GetEndpoint();
        var rateLimitAttribute = endpoint?.Metadata.GetMetadata<EndpointRateLimitAttribute>();

        if (rateLimitAttribute?.Enabled != true)
        {
            return await next(context);
        }

        var identifier = GetIdentifier(context.HttpContext, rateLimitAttribute.IdentifierType);
        if (string.IsNullOrEmpty(identifier))
        {
            return await next(context);
        }

        var isAllowed = await _rateLimitService.IsAllowedAsync(
            identifier,
            rateLimitAttribute.MaxAttempts,
            TimeSpan.FromMinutes(rateLimitAttribute.WindowMinutes)
        );

        if (!isAllowed)
        {
            context.HttpContext.Response.StatusCode = 429;
            return Results.Json(ApiResponse.Failure(rateLimitAttribute.ErrorMessage));
        }

        var result = await next(context);

        // Increment on error responses
        if (context.HttpContext.Response.StatusCode >= 400)
        {
            await _rateLimitService.IncrementAsync(
                identifier,
                TimeSpan.FromMinutes(rateLimitAttribute.WindowMinutes)
            );
        }

        return result;
    }

    private string? GetIdentifier(HttpContext context, string identifierType)
    {
        return identifierType.ToLowerInvariant() switch
        {
            "ipaddress" => context.Connection.RemoteIpAddress?.ToString(),
            "user" => context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            "email" => context.User.FindFirst(ClaimTypes.Email)?.Value,
            _ => context.Connection.RemoteIpAddress?.ToString(),
        };
    }
}
