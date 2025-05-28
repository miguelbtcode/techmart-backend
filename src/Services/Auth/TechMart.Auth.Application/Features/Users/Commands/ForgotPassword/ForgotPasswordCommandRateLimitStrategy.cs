using TechMart.Auth.Application.Common.Decorators;
using TechMart.Auth.Domain.Primitives;

namespace TechMart.Auth.Application.Features.Users.Commands.ForgotPassword;

/// <summary>
/// Strategy for forgot password command rate limiting
/// </summary>
public sealed class ForgotPasswordCommandRateLimitStrategy
    : IRateLimitStrategy<ForgotPasswordCommand>
{
    public Task<string?> GetIdentifierAsync(
        ForgotPasswordCommand command,
        CancellationToken cancellationToken
    )
    {
        return Task.FromResult(command.Email?.ToLowerInvariant());
    }

    public Task<RateLimitConfig> GetConfigurationAsync(
        ForgotPasswordCommand command,
        CancellationToken cancellationToken
    )
    {
        var config = new RateLimitConfig
        {
            MaxAttempts = 3,
            Window = TimeSpan.FromHours(1),
            ErrorMessage = "Too many password reset requests for this email address.",
        };

        return Task.FromResult(config);
    }

    public Task<RateLimitAction> GetPostExecutionActionAsync(
        ForgotPasswordCommand command,
        Result result,
        CancellationToken cancellationToken
    )
    {
        // Always increment to prevent email spam
        return Task.FromResult(RateLimitAction.Increment);
    }
}
