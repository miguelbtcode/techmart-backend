using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechMart.Product.Infrastructure.Data;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Api.Controllers;

[Route("api/[controller]")]
[Produces("application/json")]
public class HealthController(ApplicationDbContext context, ILogger<HealthController> logger)
    : BaseApiController
{
    /// <summary>
    /// Basic health check endpoint
    /// </summary>
    /// <returns>Health status</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    public ActionResult<ApiResponse<object>> GetHealth()
    {
        return Ok(ApiResponse<object>.SuccessResponse(new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Service = "TechMart Product API",
            Version = "1.0.0"
        }));
    }

    /// <summary>
    /// Detailed health check including database connectivity
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Detailed health status</returns>
    [HttpGet("detailed")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 503)]
    public async Task<ActionResult<ApiResponse<object>>> GetDetailedHealth(CancellationToken cancellationToken = default)
    {
        var healthStatus = new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Service = "TechMart Product API",
            Version = "1.0.0",
            Database = "Unknown",
            DatabaseConnected = false
        };

        try
        {
            // Test database connectivity
            var canConnect = await context.Database.CanConnectAsync(cancellationToken);
            
            var detailedStatus = new
            {
                Status = canConnect ? "Healthy" : "Unhealthy",
                Timestamp = DateTime.UtcNow,
                Service = "TechMart Product API",
                Version = "1.0.0",
                Database = context.Database.GetDbConnection().Database,
                DatabaseConnected = canConnect
            };

            return canConnect 
                ? Ok(ApiResponse<object>.SuccessResponse(detailedStatus))
                : StatusCode(503, ApiResponse<object>.FailureResponse("Service Unavailable", new[] { "Database connection failed" }));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Health check failed");
            
            return StatusCode(503, ApiResponse<object>.FailureResponse("Service Unavailable", new[] { "Health check failed" }));
        }
    }

    /// <summary>
    /// Readiness probe for Kubernetes
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Readiness status</returns>
    [HttpGet("ready")]
    [ProducesResponseType(200)]
    [ProducesResponseType(503)]
    public async Task<ActionResult> GetReadiness(CancellationToken cancellationToken = default)
    {
        try
        {
            var canConnect = await context.Database.CanConnectAsync(cancellationToken);
            return canConnect ? Ok() : StatusCode(503);
        }
        catch
        {
            return StatusCode(503);
        }
    }

    /// <summary>
    /// Liveness probe for Kubernetes
    /// </summary>
    /// <returns>Liveness status</returns>
    [HttpGet("alive")]
    [ProducesResponseType(200)]
    public ActionResult GetLiveness()
    {
        return Ok();
    }
}