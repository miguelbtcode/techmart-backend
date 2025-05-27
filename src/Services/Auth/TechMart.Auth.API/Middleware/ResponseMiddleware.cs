using System.Text.Json;
using TechMart.Auth.API.Common;
using TechMart.Auth.API.Common.Responses;

namespace TechMart.Auth.API.Middleware;

/// <summary>
/// Middleware para manejar respuestas de forma consistente
/// </summary>
public sealed class ResponseMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ResponseMiddleware> _logger;

    public ResponseMiddleware(RequestDelegate next, ILogger<ResponseMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Capturar el trace ID
        var traceId = context.TraceIdentifier;

        // Agregar trace ID a los logs
        using var logScope = _logger.BeginScope(
            new Dictionary<string, object> { ["TraceId"] = traceId }
        );

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");

            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var errorResponse = ApiResponse
                    .Failure("An internal server error occurred")
                    .WithTraceId(traceId);

                await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
            }
        }
    }
}
