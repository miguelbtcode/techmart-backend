using System.Net;
using System.Text.Json;
using TechMart.Auth.API.Common.Constants;
using TechMart.Auth.API.Common.Responses;

namespace TechMart.Auth.API.Middleware;

/// <summary>
/// Middleware para manejo global de excepciones
/// </summary>
public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger
    )
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = context.TraceIdentifier;

        _logger.LogError(exception, "Unhandled exception occurred. TraceId: {TraceId}", traceId);

        if (context.Response.HasStarted)
        {
            _logger.LogWarning("Response has already started, cannot send error response");
            return;
        }

        var response = CreateErrorResponse(exception, traceId);
        var (statusCode, apiResponse) = response;

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var json = JsonSerializer.Serialize(
            apiResponse,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
        );

        await context.Response.WriteAsync(json);
    }

    private static (int StatusCode, ApiResponse Response) CreateErrorResponse(
        Exception exception,
        string traceId
    )
    {
        return exception switch
        {
            ArgumentException or ArgumentNullException => (
                StatusCodes.Status400BadRequest,
                ApiResponse
                    .Failure(
                        ApiMessages.ValidationFailed,
                        new ApiError("INVALID_ARGUMENT", exception.Message, "Validation")
                    )
                    .WithTraceId(traceId)
            ),

            UnauthorizedAccessException => (
                StatusCodes.Status401Unauthorized,
                ApiResponse.Failure(ApiMessages.AccessDenied).WithTraceId(traceId)
            ),

            TimeoutException => (
                StatusCodes.Status408RequestTimeout,
                ApiResponse
                    .Failure(
                        "Request timeout",
                        new ApiError("TIMEOUT", "The request timed out", "Timeout")
                    )
                    .WithTraceId(traceId)
            ),

            _ => (
                StatusCodes.Status500InternalServerError,
                ApiResponse.Failure(ApiMessages.InternalServerError).WithTraceId(traceId)
            ),
        };
    }
}
