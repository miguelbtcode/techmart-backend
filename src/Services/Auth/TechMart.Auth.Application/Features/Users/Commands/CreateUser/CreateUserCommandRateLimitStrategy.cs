using TechMart.Auth.Application.Common.Decorators;
using TechMart.Auth.Application.Contracts.Context;
using TechMart.Auth.Domain.Primitives;

namespace TechMart.Auth.Application.Features.Users.Commands.CreateUser;

/// <summary>
/// Strategy for registration command rate limiting
/// </summary>
public sealed class CreateUserCommandRateLimitStrategy : IRateLimitStrategy<CreateUserCommand>
{
    private readonly ICurrentUserContext _userContext;

    public CreateUserCommandRateLimitStrategy(ICurrentUserContext userContext)
    {
        _userContext = userContext;
    }

    public Task<string?> GetIdentifierAsync(
        CreateUserCommand command,
        CancellationToken cancellationToken
    )
    {
        // Use IP address for registration attempts
        return Task.FromResult(_userContext.IpAddress);
    }

    public Task<RateLimitConfig> GetConfigurationAsync(
        CreateUserCommand command,
        CancellationToken cancellationToken
    )
    {
        var config = new RateLimitConfig
        {
            MaxAttempts = 3,
            Window = TimeSpan.FromHours(1),
            BlockDuration = TimeSpan.FromHours(24),
            ErrorMessage = "Too many registration attempts from this IP address.",
        };

        return Task.FromResult(config);
    }

    public Task<RateLimitAction> GetPostExecutionActionAsync(
        CreateUserCommand command,
        Result result,
        CancellationToken cancellationToken
    )
    {
        // Always increment for registration attempts (successful or not)
        return Task.FromResult(RateLimitAction.Increment);
    }
}
