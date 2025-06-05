using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace TechMart.SharedKernel.Behaviors;

/// <summary>
/// Pipeline behavior that logs request and response information.
/// Provides detailed logging for debugging and monitoring purposes.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var requestId = Guid.NewGuid();

        // Log request
        _logger.LogInformation(
            "Handling request {RequestName} with ID {RequestId}: {@Request}",
            requestName,
            requestId,
            request);

        var stopwatch = Stopwatch.StartNew();
        TResponse? response = default;
        Exception? exception = null;

        try
        {
            response = await next();
            return response;
        }
        catch (Exception ex)
        {
            exception = ex;
            throw;
        }
        finally
        {
            stopwatch.Stop();

            if (exception != null)
            {
                _logger.LogError(exception,
                    "Request {RequestName} with ID {RequestId} failed after {ElapsedMilliseconds}ms",
                    requestName,
                    requestId,
                    stopwatch.ElapsedMilliseconds);
            }
            else
            {
                _logger.LogInformation(
                    "Request {RequestName} with ID {RequestId} completed successfully in {ElapsedMilliseconds}ms: {@Response}",
                    requestName,
                    requestId,
                    stopwatch.ElapsedMilliseconds,
                    response);
            }
        }
    }
}

/// <summary>
/// Pipeline behavior that logs only essential information for production environments.
/// Provides minimal logging overhead while maintaining observability.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public class MinimalLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<MinimalLoggingBehavior<TRequest, TResponse>> _logger;

    public MinimalLoggingBehavior(ILogger<MinimalLoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var requestId = Guid.NewGuid();

        _logger.LogDebug("Executing request {RequestName} with ID {RequestId}", requestName, requestId);

        try
        {
            var response = await next();
            _logger.LogDebug("Request {RequestName} with ID {RequestId} completed successfully", requestName, requestId);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Request {RequestName} with ID {RequestId} failed", requestName, requestId);
            throw;
        }
    }
}