using System.Reflection;
using Microsoft.Extensions.Logging;
using TechMart.Auth.Application.Contracts.Authentication;
using TechMart.Auth.Application.Messaging.Pipeline;

namespace TechMart.Auth.Application.Common.Behaviors;

public sealed class AuthorizationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;
    private readonly ILogger<AuthorizationBehavior<TRequest, TResponse>> _logger;

    public AuthorizationBehavior(
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService,
        ILogger<AuthorizationBehavior<TRequest, TResponse>> logger
    )
    {
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        var authorizationAttributes = request
            .GetType()
            .GetCustomAttributes<AuthorizeAttribute>()
            .ToList();

        if (!authorizationAttributes.Any())
        {
            return await next();
        }

        var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
        if (currentUser == null)
        {
            _logger.LogWarning(
                "Authorization failed: No current user found for request {RequestType}",
                typeof(TRequest).Name
            );
            throw new UnauthorizedAccessException("User is not authenticated");
        }

        foreach (var authAttribute in authorizationAttributes)
        {
            // Check roles
            if (!string.IsNullOrEmpty(authAttribute.Roles))
            {
                var requiredRoles = authAttribute
                    .Roles.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(r => r.Trim())
                    .ToList();

                var hasRequiredRole = requiredRoles.Any(role =>
                    currentUser.Roles.Contains(role, StringComparer.OrdinalIgnoreCase)
                );

                if (!hasRequiredRole)
                {
                    _logger.LogWarning(
                        "Authorization failed: User {UserId} does not have required roles {RequiredRoles} for {RequestType}",
                        currentUser.Id,
                        string.Join(", ", requiredRoles),
                        typeof(TRequest).Name
                    );
                    throw new ForbiddenAccessException(
                        $"User does not have required roles: {string.Join(", ", requiredRoles)}"
                    );
                }
            }

            // Check policy
            if (!string.IsNullOrEmpty(authAttribute.Policy))
            {
                var authResult = await _authorizationService.AuthorizeAsync(
                    currentUser.ToPrincipal(),
                    authAttribute.Policy
                );

                if (!authResult.Succeeded)
                {
                    _logger.LogWarning(
                        "Authorization failed: User {UserId} does not satisfy policy {Policy} for {RequestType}",
                        currentUser.Id,
                        authAttribute.Policy,
                        typeof(TRequest).Name
                    );
                    throw new ForbiddenAccessException(
                        $"User does not satisfy policy: {authAttribute.Policy}"
                    );
                }
            }
        }

        _logger.LogDebug(
            "Authorization successful for user {UserId} on {RequestType}",
            currentUser.Id,
            typeof(TRequest).Name
        );

        return await next();
    }
}
