using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using TechMart.SharedKernel.Common;
using TechMart.SharedKernel.Exceptions;

namespace TechMart.SharedKernel.Extensions.ApplicationBuilder;

public static class ExceptionHandlingExtensions
{
    public static IApplicationBuilder UseTechMartExceptionHandling(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                var loggerFactory = context.RequestServices.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger("TechMart.GlobalExceptionHandler");
                
                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                
                if (contextFeature?.Error != null)
                {
                    var exception = contextFeature.Error;
                    var traceId = context.TraceIdentifier;

                    logger.LogError(exception, 
                        "Global exception handler caught unhandled exception. TraceId: {TraceId}", traceId);

                    var (statusCode, response) = MapExceptionToResponse(exception, traceId);

                    context.Response.StatusCode = (int)statusCode;
                    context.Response.ContentType = "application/json";

                    var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        WriteIndented = false
                    });

                    await context.Response.WriteAsync(jsonResponse);
                }
            });
        });

        return app;
    }

    public static IApplicationBuilder UseTechMartExceptionHandling(this IApplicationBuilder app, bool isDevelopment)
    {
        if (isDevelopment)
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseTechMartExceptionHandling();
        }

        return app;
    }

    private static (HttpStatusCode statusCode, ApiResponse response) MapExceptionToResponse(Exception exception, string traceId)
    {
        return exception switch
        {
            ValidationException validationEx => (
                HttpStatusCode.BadRequest,
                ApiResponse.FailureResponse(
                    "Validation failed",
                    validationEx.Errors.SelectMany(e => e.Value),
                    traceId)),

            NotFoundException notFoundEx => (
                HttpStatusCode.NotFound,
                ApiResponse.FailureResponse(notFoundEx.ToError(), traceId)),

            ConflictException conflictEx => (
                HttpStatusCode.Conflict,
                ApiResponse.FailureResponse(conflictEx.ToError(), traceId)),

            UnauthorizedException unauthorizedEx => (
                HttpStatusCode.Unauthorized,
                ApiResponse.FailureResponse(unauthorizedEx.ToError(), traceId)),

            DomainException domainEx => (
                HttpStatusCode.BadRequest,
                ApiResponse.FailureResponse(
                    Error.Failure(domainEx.ErrorCode, domainEx.Message),
                    traceId)),

            _ => (
                HttpStatusCode.InternalServerError,
                ApiResponse.FailureResponse(
                    "An unexpected error occurred",
                    new[] { "Internal server error" },
                    traceId))
        };
    }
}