namespace TechMart.Auth.API.Common.Responses;

/// <summary>
/// Response model for health checks
/// </summary>
public class HealthCheckResponse
{
    public string Status { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public double Duration { get; set; }
    public List<HealthCheckItem> Checks { get; set; } = [];
}

/// Individual health check item
/// </summary>
public class HealthCheckItem
{
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public double Duration { get; set; }
    public string? Description { get; set; }
    public string? Exception { get; set; }
}

/// <summary>
/// Response model for liveness check
/// </summary>
public class LivenessResponse
{
    public string Status { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
