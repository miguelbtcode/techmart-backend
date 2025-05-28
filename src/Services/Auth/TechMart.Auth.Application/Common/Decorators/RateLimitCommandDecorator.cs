using Microsoft.Extensions.Logging;
using TechMart.Auth.Application.Contracts.Infrastructure;
using TechMart.Auth.Application.Messaging.Commands;
using TechMart.Auth.Domain.Primitives;

namespace TechMart.Auth.Application.Common.Decorators;

/// <summary>
/// Decorator for rate limiting specific commands
/// Provides targeted rate limiting with fine-grained control
/// </summary>
public sealed class RateLimitCommandDecorator<TCommand> : ICommandHandler<TCommand>
    where TCommand : ICommand
{
    private readonly ICommandHandler<TCommand> _handler;
    private readonly IRateLimitService _rateLimitService;
    private readonly IRateLimitStrategy<TCommand> _strategy;
    private readonly ILogger<RateLimitCommandDecorator<TCommand>> _logger;

    public RateLimitCommandDecorator(
        ICommandHandler<TCommand> handler,
        IRateLimitService rateLimitService,
        IRateLimitStrategy<TCommand> strategy,
        ILogger<RateLimitCommandDecorator<TCommand>> logger
    )
    {
        _handler = handler;
        _rateLimitService = rateLimitService;
        _strategy = strategy;
        _logger = logger;
    }

    public async Task<Result> Handle(TCommand command, CancellationToken cancellationToken)
    {
        // Pre-execution rate limit check
        var preCheckResult = await PerformPreExecutionCheck(command, cancellationToken);
        if (preCheckResult.IsFailure)
        {
            return preCheckResult;
        }

        // Execute the command
        var result = await _handler.Handle(command, cancellationToken);

        // Post-execution actions
        await PerformPostExecutionActions(command, result, cancellationToken);

        return result;
    }

    private async Task<Result> PerformPreExecutionCheck(
        TCommand command,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var identifier = await _strategy.GetIdentifierAsync(command, cancellationToken);
            if (string.IsNullOrEmpty(identifier))
            {
                _logger.LogWarning(
                    "No identifier found for rate limiting {CommandType}",
                    typeof(TCommand).Name
                );
                return Result.Success();
            }

            var config = await _strategy.GetConfigurationAsync(command, cancellationToken);

            var isAllowed = await _rateLimitService.IsAllowedAsync(
                identifier,
                config.MaxAttempts,
                config.Window,
                cancellationToken
            );

            if (!isAllowed)
            {
                _logger.LogWarning(
                    "Rate limit exceeded for {CommandType} with identifier {Identifier}",
                    typeof(TCommand).Name,
                    identifier
                );

                return Result.Failure(Error.Failure("RATE_LIMIT_EXCEEDED", config.ErrorMessage));
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Pre-execution rate limit check failed for {CommandType}",
                typeof(TCommand).Name
            );
            // Don't fail the command due to rate limiting errors
            return Result.Success();
        }
    }

    private async Task PerformPostExecutionActions(
        TCommand command,
        Result result,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var identifier = await _strategy.GetIdentifierAsync(command, cancellationToken);
            if (string.IsNullOrEmpty(identifier))
            {
                return;
            }

            var config = await _strategy.GetConfigurationAsync(command, cancellationToken);
            var action = await _strategy.GetPostExecutionActionAsync(
                command,
                result,
                cancellationToken
            );

            switch (action)
            {
                case RateLimitAction.Increment:
                    await _rateLimitService.IncrementAsync(
                        identifier,
                        config.Window,
                        cancellationToken
                    );
                    _logger.LogDebug(
                        "Rate limit incremented for {CommandType}",
                        typeof(TCommand).Name
                    );
                    break;

                case RateLimitAction.Reset:
                    await _rateLimitService.ResetAsync(identifier, cancellationToken);
                    _logger.LogDebug("Rate limit reset for {CommandType}", typeof(TCommand).Name);
                    break;

                case RateLimitAction.Block:
                    await _rateLimitService.BlockAsync(
                        identifier,
                        config.BlockDuration ?? TimeSpan.FromHours(1),
                        cancellationToken
                    );
                    _logger.LogWarning(
                        "Identifier {Identifier} blocked due to {CommandType}",
                        identifier,
                        typeof(TCommand).Name
                    );
                    break;

                case RateLimitAction.None:
                default:
                    // Do nothing
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Post-execution rate limit action failed for {CommandType}",
                typeof(TCommand).Name
            );
            // Don't fail the command due to rate limiting errors
        }
    }
}

/// <summary>
/// Decorator for commands with return values
/// </summary>
public sealed class RateLimitCommandDecorator<TCommand, TResponse>
    : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    private readonly ICommandHandler<TCommand, TResponse> _handler;
    private readonly IRateLimitService _rateLimitService;
    private readonly IRateLimitStrategy<TCommand> _strategy;
    private readonly ILogger<RateLimitCommandDecorator<TCommand, TResponse>> _logger;

    public RateLimitCommandDecorator(
        ICommandHandler<TCommand, TResponse> handler,
        IRateLimitService rateLimitService,
        IRateLimitStrategy<TCommand> strategy,
        ILogger<RateLimitCommandDecorator<TCommand, TResponse>> logger
    )
    {
        _handler = handler;
        _rateLimitService = rateLimitService;
        _strategy = strategy;
        _logger = logger;
    }

    public async Task<Result<TResponse>> Handle(
        TCommand command,
        CancellationToken cancellationToken
    )
    {
        // Pre-execution rate limit check
        var preCheckResult = await PerformPreExecutionCheck(command, cancellationToken);
        if (preCheckResult.IsFailure)
        {
            return Result.Failure<TResponse>(preCheckResult.Error);
        }

        // Execute the command
        var result = await _handler.Handle(command, cancellationToken);

        // Post-execution actions
        await PerformPostExecutionActions(command, result, cancellationToken);

        return result;
    }

    private async Task<Result> PerformPreExecutionCheck(
        TCommand command,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var identifier = await _strategy.GetIdentifierAsync(command, cancellationToken);
            if (string.IsNullOrEmpty(identifier))
            {
                return Result.Success();
            }

            var config = await _strategy.GetConfigurationAsync(command, cancellationToken);

            var isAllowed = await _rateLimitService.IsAllowedAsync(
                identifier,
                config.MaxAttempts,
                config.Window,
                cancellationToken
            );

            if (!isAllowed)
            {
                _logger.LogWarning(
                    "Rate limit exceeded for {CommandType} with identifier {Identifier}",
                    typeof(TCommand).Name,
                    identifier
                );

                return Result.Failure(Error.Failure("RATE_LIMIT_EXCEEDED", config.ErrorMessage));
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Pre-execution rate limit check failed for {CommandType}",
                typeof(TCommand).Name
            );
            return Result.Success();
        }
    }

    private async Task PerformPostExecutionActions<T>(
        TCommand command,
        Result<T> result,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var identifier = await _strategy.GetIdentifierAsync(command, cancellationToken);
            if (string.IsNullOrEmpty(identifier))
            {
                return;
            }

            var config = await _strategy.GetConfigurationAsync(command, cancellationToken);
            var action = await _strategy.GetPostExecutionActionAsync(
                command,
                result,
                cancellationToken
            );

            switch (action)
            {
                case RateLimitAction.Increment:
                    await _rateLimitService.IncrementAsync(
                        identifier,
                        config.Window,
                        cancellationToken
                    );
                    break;

                case RateLimitAction.Reset:
                    await _rateLimitService.ResetAsync(identifier, cancellationToken);
                    break;

                case RateLimitAction.Block:
                    await _rateLimitService.BlockAsync(
                        identifier,
                        config.BlockDuration ?? TimeSpan.FromHours(1),
                        cancellationToken
                    );
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Post-execution rate limit action failed for {CommandType}",
                typeof(TCommand).Name
            );
        }
    }
}

/// <summary>
/// Strategy interface for rate limiting logic
/// </summary>
public interface IRateLimitStrategy<TCommand>
{
    Task<string?> GetIdentifierAsync(TCommand command, CancellationToken cancellationToken);
    Task<RateLimitConfig> GetConfigurationAsync(
        TCommand command,
        CancellationToken cancellationToken
    );
    Task<RateLimitAction> GetPostExecutionActionAsync(
        TCommand command,
        Result result,
        CancellationToken cancellationToken
    );
}

/// <summary>
/// Configuration for rate limiting
/// </summary>
public record RateLimitConfig
{
    public int MaxAttempts { get; init; } = 5;
    public TimeSpan Window { get; init; } = TimeSpan.FromMinutes(15);
    public TimeSpan? BlockDuration { get; init; }
    public string ErrorMessage { get; init; } = "Rate limit exceeded. Please try again later.";
}

/// <summary>
/// Actions to take after command execution
/// </summary>
public enum RateLimitAction
{
    None,
    Increment,
    Reset,
    Block,
}
