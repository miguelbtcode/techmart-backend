using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using TechMart.Auth.Application.Contracts.Authentication;
using TechMart.Auth.Application.Messaging.Pipeline;
using TechMart.Auth.Domain.Primitives;

namespace TechMart.Auth.Application.Common.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    private readonly ICurrentUserService _currentUserService;

    public LoggingBehavior(
        ILogger<LoggingBehavior<TRequest, TResponse>> logger,
        ICurrentUserService currentUserService
    )
    {
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        var requestName = typeof(TRequest).Name;
        var requestType = typeof(TRequest).BaseType?.Name ?? "Request";
        var userId = await _currentUserService.GetCurrentUserIdAsync(cancellationToken);
        var requestId = Guid.NewGuid();

        using var scope = LogContext.PushProperty("RequestId", requestId);
        using var userScope = LogContext.PushProperty("UserId", userId);
        using var requestScope = LogContext.PushProperty("RequestType", requestType);

        _logger.LogInformation(
            "Starting {RequestType} {RequestName} for user {UserId}",
            requestType,
            requestName,
            userId
        );

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var response = await next();
            stopwatch.Stop();

            if (response is Result result)
            {
                if (result.IsFailure)
                {
                    _logger.LogWarning(
                        "{RequestType} {RequestName} failed in {ElapsedMs}ms with error: {Error}",
                        requestType,
                        requestName,
                        stopwatch.ElapsedMilliseconds,
                        result.Error
                    );
                }
                else
                {
                    _logger.LogInformation(
                        "{RequestType} {RequestName} completed successfully in {ElapsedMs}ms",
                        requestType,
                        requestName,
                        stopwatch.ElapsedMilliseconds
                    );
                }
            }
            else
            {
                _logger.LogInformation(
                    "{RequestType} {RequestName} completed in {ElapsedMs}ms",
                    requestType,
                    requestName,
                    stopwatch.ElapsedMilliseconds
                );
            }

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(
                ex,
                "{RequestType} {RequestName} failed in {ElapsedMs}ms with exception",
                requestType,
                requestName,
                stopwatch.ElapsedMilliseconds
            );
            throw;
        }
    }
}
