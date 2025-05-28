using TechMart.Auth.Application.Common.Decorators;
using TechMart.Auth.Application.Contracts.Context;
using TechMart.Auth.Domain.Primitives;

namespace TechMart.Auth.Application.Features.Authentication.Commands.Login;

/// <summary>
/// Strategy for login command rate limiting
/// </summary>
public sealed class LoginCommandRateLimitStrategy : IRateLimitStrategy<LoginCommand>
{
    private readonly ICurrentUserContext _userContext;

    public LoginCommandRateLimitStrategy(ICurrentUserContext userContext)
    {
        _userContext = userContext;
    }

    public Task<string?> GetIdentifierAsync(
        LoginCommand command,
        CancellationToken cancellationToken
    )
    {
        // Use email as primary identifier, fallback to IP
        var identifier = !string.IsNullOrEmpty(command.Email)
            ? command.Email.ToLowerInvariant()
            : _userContext.IpAddress;

        return Task.FromResult(identifier);
    }

    public Task<RateLimitConfig> GetConfigurationAsync(
        LoginCommand command,
        CancellationToken cancellationToken
    )
    {
        var config = new RateLimitConfig
        {
            MaxAttempts = 5,
            Window = TimeSpan.FromMinutes(15),
            BlockDuration = TimeSpan.FromHours(1),
            ErrorMessage = "Too many login attempts. Please wait 15 minutes before trying again.",
        };

        return Task.FromResult(config);
    }

    public Task<RateLimitAction> GetPostExecutionActionAsync(
        LoginCommand command,
        Result result,
        CancellationToken cancellationToken
    )
    {
        // Increment on failure, reset on success
        var action = result.IsFailure ? RateLimitAction.Increment : RateLimitAction.Reset;
        return Task.FromResult(action);
    }
}
