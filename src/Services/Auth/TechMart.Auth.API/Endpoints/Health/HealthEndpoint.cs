using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using TechMart.Auth.API.Common.Constants;
using TechMart.Auth.API.Common.Responses;

namespace TechMart.Auth.API.Endpoints.Health;

/// <summary>
/// Endpoints para health checks
/// </summary>
public sealed class HealthEndpoint : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        var healthGroup = app.MapGroup(ApiRoutes.Health.Base).WithTags(ApiTags.Health);

        // Basic health check
        healthGroup
            .MapGet("", HandleHealthAsync)
            .WithName("Health")
            .WithSummary("Basic health check")
            .WithDescription("Returns the basic health status of the API")
            .Produces<ApiResponse<HealthCheckResponse>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<HealthCheckResponse>>(StatusCodes.Status503ServiceUnavailable)
            .WithOpenApi();

        // Readiness check
        healthGroup
            .MapGet(ApiRoutes.Health.Ready, HandleReadinessAsync)
            .WithName("HealthReady")
            .WithSummary("Readiness health check")
            .WithDescription(
                "Returns readiness status including database and external dependencies"
            )
            .Produces<ApiResponse<HealthCheckResponse>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<HealthCheckResponse>>(StatusCodes.Status503ServiceUnavailable)
            .WithOpenApi();

        // Liveness check
        healthGroup
            .MapGet(ApiRoutes.Health.Live, HandleLivenessAsync)
            .WithName("HealthLive")
            .WithSummary("Liveness health check")
            .WithDescription("Returns liveness status of the API")
            .Produces<ApiResponse<LivenessResponse>>(StatusCodes.Status200OK)
            .WithOpenApi();
    }

    private static async Task<
        Results<
            Ok<ApiResponse<HealthCheckResponse>>,
            JsonHttpResult<ApiResponse<HealthCheckResponse>>
        >
    > HandleHealthAsync([FromServices] HealthCheckService healthCheckService, HttpContext context)
    {
        try
        {
            var report = await healthCheckService.CheckHealthAsync();

            var healthData = new HealthCheckResponse
            {
                Status = report.Status.ToString(),
                Timestamp = DateTime.UtcNow,
                Duration = report.TotalDuration.TotalMilliseconds,
                Checks = report
                    .Entries.Select(entry => new HealthCheckItem
                    {
                        Name = entry.Key,
                        Status = entry.Value.Status.ToString(),
                        Duration = entry.Value.Duration.TotalMilliseconds,
                        Description = entry.Value.Description,
                        Exception = entry.Value.Exception?.Message,
                    })
                    .ToList(),
            };

            var response =
                report.Status == HealthStatus.Healthy
                    ? ApiResponse<HealthCheckResponse>.Success(healthData, "API is healthy")
                    : ApiResponse<HealthCheckResponse>.Failure(
                        "API is unhealthy",
                        new ApiError(
                            "HEALTH_CHECK_FAILED",
                            "One or more health checks failed",
                            "Health"
                        )
                    );

            response.WithTraceId(context.TraceIdentifier);

            return report.Status == HealthStatus.Healthy
                ? TypedResults.Ok(response)
                : TypedResults.Json(response, statusCode: StatusCodes.Status503ServiceUnavailable);
        }
        catch (Exception ex)
        {
            var errorResponse = ApiResponse<HealthCheckResponse>
                .Failure(
                    "Health check failed",
                    new ApiError("HEALTH_CHECK_ERROR", ex.Message, "Health")
                )
                .WithTraceId(context.TraceIdentifier);

            return TypedResults.Json(
                errorResponse,
                statusCode: StatusCodes.Status503ServiceUnavailable
            );
        }
    }

    private static async Task<
        Results<
            Ok<ApiResponse<HealthCheckResponse>>,
            JsonHttpResult<ApiResponse<HealthCheckResponse>>
        >
    > HandleReadinessAsync(
        [FromServices] HealthCheckService healthCheckService,
        HttpContext context
    )
    {
        try
        {
            var report = await healthCheckService.CheckHealthAsync(c => c.Tags.Contains("ready"));

            var healthData = new HealthCheckResponse
            {
                Status = report.Status.ToString(),
                Timestamp = DateTime.UtcNow,
                Duration = report.TotalDuration.TotalMilliseconds,
                Checks = report
                    .Entries.Select(entry => new HealthCheckItem
                    {
                        Name = entry.Key,
                        Status = entry.Value.Status.ToString(),
                        Duration = entry.Value.Duration.TotalMilliseconds,
                        Description = entry.Value.Description,
                        Exception = entry.Value.Exception?.Message,
                    })
                    .ToList(),
            };

            var response =
                report.Status == HealthStatus.Healthy
                    ? ApiResponse<HealthCheckResponse>.Success(healthData, "API is ready")
                    : ApiResponse<HealthCheckResponse>.Failure(
                        "API is not ready",
                        new ApiError(
                            "READINESS_CHECK_FAILED",
                            "One or more readiness checks failed",
                            "Health"
                        )
                    );

            response.WithTraceId(context.TraceIdentifier);

            return report.Status == HealthStatus.Healthy
                ? TypedResults.Ok(response)
                : TypedResults.Json(response, statusCode: StatusCodes.Status503ServiceUnavailable);
        }
        catch (Exception ex)
        {
            var errorResponse = ApiResponse<HealthCheckResponse>
                .Failure(
                    "Readiness check failed",
                    new ApiError("READINESS_CHECK_ERROR", ex.Message, "Health")
                )
                .WithTraceId(context.TraceIdentifier);

            return TypedResults.Json(
                errorResponse,
                statusCode: StatusCodes.Status503ServiceUnavailable
            );
        }
    }

    private static Task<IResult> HandleLivenessAsync(
        [FromServices] HealthCheckService healthCheckService,
        HttpContext context
    )
    {
        var healthData = new LivenessResponse { Status = "Alive", Timestamp = DateTime.UtcNow };

        var response = ApiResponse<LivenessResponse>
            .Success(healthData, "API is alive")
            .WithTraceId(context.TraceIdentifier);

        return Task.FromResult(Results.Ok(response));
    }
}
