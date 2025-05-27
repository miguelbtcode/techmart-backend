using System.Text.Json;
using FluentValidation;
using TechMart.Auth.API.Common.Responses;

namespace TechMart.Auth.API.Middleware;

/// <summary>
/// Middleware para manejo centralizado de validaciones
/// </summary>
public sealed class ValidationMiddleware
{
    private readonly RequestDelegate _next;

    public ValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            var errors = ex
                .Errors.Select(e => new ApiError(
                    e.PropertyName,
                    string.Join(", ", e.ErrorMessage),
                    "Validation",
                    e.PropertyName
                ))
                .ToArray();

            var response = ApiResponse
                .Failure("Validation failed", errors)
                .WithTraceId(context.TraceIdentifier);

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
