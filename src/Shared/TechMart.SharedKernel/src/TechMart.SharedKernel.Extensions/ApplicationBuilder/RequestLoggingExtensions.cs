using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace TechMart.SharedKernel.Extensions.ApplicationBuilder;

/// <summary>
/// Extension methods for configuring request logging.
/// </summary>
public static class RequestLoggingExtensions
{
    /// <summary>
    /// Adds request logging middleware.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The application builder.</returns>
    public static IApplicationBuilder UseTechMartRequestLogging(this IApplicationBuilder app)
    {
        app.Use(async (context, next) =>
        {
            var loggerFactory = context.RequestServices.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("TechMart.RequestLogging");
            var stopwatch = Stopwatch.StartNew();
            var traceId = context.TraceIdentifier;

            logger.LogInformation(
                "Request started: {Method} {Path} - TraceId: {TraceId}",
                context.Request.Method,
                context.Request.Path,
                traceId);

            try
            {
                await next();
            }
            finally
            {
                stopwatch.Stop();
                
                logger.LogInformation(
                    "Request completed: {Method} {Path} - Status: {StatusCode} - Duration: {ElapsedMs}ms - TraceId: {TraceId}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds,
                    traceId);
            }
        });

        return app;
    }

    /// <summary>
    /// Adds detailed request logging with headers and body (for development).
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The application builder.</returns>
    public static IApplicationBuilder UseTechMartDetailedRequestLogging(this IApplicationBuilder app)
    {
        app.Use(async (context, next) =>
        {
            var loggerFactory = context.RequestServices.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("TechMart.DetailedRequestLogging");
            var stopwatch = Stopwatch.StartNew();
            var traceId = context.TraceIdentifier;

            // Log request details
            logger.LogInformation(
                "Request started: {Method} {Path} - Headers: {@Headers} - TraceId: {TraceId}",
                context.Request.Method,
                context.Request.Path,
                context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
                traceId);

            try
            {
                await next();
            }
            finally
            {
                stopwatch.Stop();
                
                logger.LogInformation(
                    "Request completed: {Method} {Path} - Status: {StatusCode} - Duration: {ElapsedMs}ms - TraceId: {TraceId}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds,
                    traceId);
            }
        });

        return app;
    }

    /// <summary>
    /// Adds request correlation ID middleware.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <param name="headerName">The correlation ID header name.</param>
    /// <returns>The application builder.</returns>
    public static IApplicationBuilder UseTechMartCorrelationId(
        this IApplicationBuilder app,
        string headerName = "X-Correlation-ID")
    {
        app.Use(async (context, next) =>
        {
            var correlationId = context.Request.Headers[headerName].FirstOrDefault() ?? Guid.NewGuid().ToString();
            
            context.Items["CorrelationId"] = correlationId;
            context.Response.Headers[headerName] = correlationId;

            await next();
        });

        return app;
    }

    /// <summary>
    /// Adds performance logging middleware that warns about slow requests.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <param name="slowRequestThresholdMs">Threshold in milliseconds for slow requests.</param>
    /// <returns>The application builder.</returns>
    public static IApplicationBuilder UseTechMartPerformanceLogging(
        this IApplicationBuilder app,
        int slowRequestThresholdMs = 5000)
    {
        app.Use(async (context, next) =>
        {
            var loggerFactory = context.RequestServices.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("TechMart.PerformanceLogging");
            var stopwatch = Stopwatch.StartNew();
            var traceId = context.TraceIdentifier;

            try
            {
                await next();
            }
            finally
            {
                stopwatch.Stop();
                var elapsedMs = stopwatch.ElapsedMilliseconds;

                if (elapsedMs > slowRequestThresholdMs)
                {
                    logger.LogWarning(
                        "Slow request detected: {Method} {Path} took {ElapsedMs}ms - TraceId: {TraceId}",
                        context.Request.Method,
                        context.Request.Path,
                        elapsedMs,
                        traceId);
                }
                else
                {
                    logger.LogDebug(
                        "Request performance: {Method} {Path} - {ElapsedMs}ms - TraceId: {TraceId}",
                        context.Request.Method,
                        context.Request.Path,
                        elapsedMs,
                        traceId);
                }
            }
        });

        return app;
    }

    /// <summary>
    /// Adds request/response body logging (use with caution in production).
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <param name="maxBodySize">Maximum body size to log in bytes.</param>
    /// <returns>The application builder.</returns>
    public static IApplicationBuilder UseTechMartBodyLogging(
        this IApplicationBuilder app,
        int maxBodySize = 4096)
    {
        app.Use(async (context, next) =>
        {
            var loggerFactory = context.RequestServices.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("TechMart.BodyLogging");
            var traceId = context.TraceIdentifier;

            // Log request body
            if (context.Request.ContentLength.HasValue && 
                context.Request.ContentLength.Value > 0 && 
                context.Request.ContentLength.Value <= maxBodySize)
            {
                context.Request.EnableBuffering();
                var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
                context.Request.Body.Position = 0;

                logger.LogDebug(
                    "Request body: {Method} {Path} - Body: {RequestBody} - TraceId: {TraceId}",
                    context.Request.Method,
                    context.Request.Path,
                    requestBody,
                    traceId);
            }

            await next();
        });

        return app;
    }
}