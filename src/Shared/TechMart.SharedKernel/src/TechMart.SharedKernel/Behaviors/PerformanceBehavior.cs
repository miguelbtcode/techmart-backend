using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace TechMart.SharedKernel.Behaviors;

/// <summary>
/// Pipeline behavior that monitors and logs request performance.
/// Warns about slow requests and provides performance metrics.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
    private readonly int _slowRequestThresholdMs;

    public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
        _slowRequestThresholdMs = 5000; // 5 seconds default threshold
    }

    public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger, int slowRequestThresholdMs)
    {
        _logger = logger;
        _slowRequestThresholdMs = slowRequestThresholdMs;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var response = await next();
            
            stopwatch.Stop();
            var elapsedMs = stopwatch.ElapsedMilliseconds;

            if (elapsedMs > _slowRequestThresholdMs)
            {
                _logger.LogWarning(
                    "Slow request detected: {RequestName} took {ElapsedMilliseconds}ms to complete. Request: {@Request}",
                    requestName,
                    elapsedMs,
                    request);
            }
            else
            {
                _logger.LogInformation(
                    "Request {RequestName} completed in {ElapsedMilliseconds}ms",
                    requestName,
                    elapsedMs);
            }

            return response;
        }
        catch (Exception)
        {
            stopwatch.Stop();
            _logger.LogWarning(
                "Request {RequestName} failed after {ElapsedMilliseconds}ms",
                requestName,
                stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}

/// <summary>
/// Pipeline behavior that collects detailed performance metrics.
/// Suitable for development and testing environments.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public class DetailedPerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<DetailedPerformanceBehavior<TRequest, TResponse>> _logger;

    public DetailedPerformanceBehavior(ILogger<DetailedPerformanceBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var requestId = Guid.NewGuid();

        // Capture memory before execution
        var memoryBefore = GC.GetTotalMemory(false);
        var stopwatch = Stopwatch.StartNew();

        _logger.LogDebug(
            "Starting request {RequestName} with ID {RequestId}. Memory before: {MemoryBefore} bytes",
            requestName,
            requestId,
            memoryBefore);

        try
        {
            var response = await next();
            
            stopwatch.Stop();
            var memoryAfter = GC.GetTotalMemory(false);
            var memoryUsed = memoryAfter - memoryBefore;

            _logger.LogInformation(
                "Request {RequestName} with ID {RequestId} completed successfully. " +
                "Duration: {ElapsedMilliseconds}ms, Memory used: {MemoryUsed} bytes",
                requestName,
                requestId,
                stopwatch.ElapsedMilliseconds,
                memoryUsed);

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            var memoryAfter = GC.GetTotalMemory(false);
            var memoryUsed = memoryAfter - memoryBefore;

            _logger.LogError(ex,
                "Request {RequestName} with ID {RequestId} failed. " +
                "Duration: {ElapsedMilliseconds}ms, Memory used: {MemoryUsed} bytes",
                requestName,
                requestId,
                stopwatch.ElapsedMilliseconds,
                memoryUsed);

            throw;
        }
    }
}