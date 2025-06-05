using System.Net;
using System.Text.Json;
using TechMart.Auth.Application.Common.Results;
using FluentValidation;

namespace TechMart.Auth.Api.Middlewares;

public class GlobalExceptionMiddleware
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
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var result = Result.Failure("An error occurred while processing your request.");

        switch (exception)
        {
            case ValidationException validationEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                result = CreateValidationErrorResult(validationEx);
                break;

            case ArgumentException argEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                result = Result.Failure(argEx.Message);
                break;

            case UnauthorizedAccessException:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                result = Result.Failure("Unauthorized access.");
                break;

            case KeyNotFoundException:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                result = Result.Failure("Resource not found.");
                break;

            case InvalidOperationException invalidOpEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                result = Result.Failure(invalidOpEx.Message);
                break;

            case TimeoutException:
                context.Response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                result = Result.Failure("The request timed out. Please try again.");
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                result = Result.Failure("An unexpected error occurred. Please try again later.");
                break;
        }

        var jsonResult = JsonSerializer.Serialize(
            result,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
        );

        await context.Response.WriteAsync(jsonResult);
    }

    private static Result CreateValidationErrorResult(ValidationException validationException)
    {
        var errors = validationException
            .Errors.GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

        return Result.Failure("Validation failed.", errors);
    }
}
