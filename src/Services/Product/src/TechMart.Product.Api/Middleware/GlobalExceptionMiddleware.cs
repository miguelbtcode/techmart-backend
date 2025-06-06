using System.Text.Json;
using TechMart.Product.Application.Exceptions;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Api.Middleware;

public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var (statusCode, apiResponse) = exception switch
        {
            ValidationException validationEx => (400, CreateValidationErrorResponse(validationEx, context)),
            ArgumentException argEx => (400, CreateArgumentErrorResponse(argEx, context)),
            UnauthorizedAccessException => (401, CreateUnauthorizedResponse(context)),
            NotImplementedException => (501, CreateNotImplementedResponse(context)),
            _ => (500, CreateInternalServerErrorResponse(exception, context))
        };

        response.StatusCode = statusCode;
        
        var jsonResponse = JsonSerializer.Serialize(apiResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await response.WriteAsync(jsonResponse);
    }

    private static ApiResponse CreateValidationErrorResponse(ValidationException ex, HttpContext context)
    {
        var errors = ex.Errors.Select(e => new
        {
            field = e.PropertyName,
            message = e.ErrorMessage
        }).ToArray();

        return ApiResponse.FailureResponse(
            "Validation failed",
            errors.Select(e => e.message),
            context.TraceIdentifier
        );
    }

    private static ApiResponse CreateArgumentErrorResponse(ArgumentException ex, HttpContext context)
    {
        return ApiResponse.FailureResponse(
            "Invalid argument",
            new[] { ex.Message },
            context.TraceIdentifier
        );
    }

    private static ApiResponse CreateUnauthorizedResponse(HttpContext context)
    {
        return ApiResponse.FailureResponse(
            "Unauthorized access",
            new[] { "You are not authorized to access this resource" },
            context.TraceIdentifier
        );
    }

    private static ApiResponse CreateNotImplementedResponse(HttpContext context)
    {
        return ApiResponse.FailureResponse(
            "Feature not implemented",
            new[] { "This feature is not yet implemented" },
            context.TraceIdentifier
        );
    }

    private static ApiResponse CreateInternalServerErrorResponse(Exception ex, HttpContext context)
    {
        return ApiResponse.FailureResponse(
            "Internal server error",
            new[] { "An unexpected error occurred. Please try again later." },
            context.TraceIdentifier
        );
    }
}