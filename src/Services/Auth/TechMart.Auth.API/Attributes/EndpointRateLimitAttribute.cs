namespace TechMart.Auth.API.Attributes;

/// <summary>
/// Attribute for endpoint-level rate limiting
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class EndpointRateLimitAttribute : Attribute
{
    public int MaxAttempts { get; set; } = 10;
    public int WindowMinutes { get; set; } = 15;
    public string IdentifierType { get; set; } = "IpAddress"; // IpAddress, User, Email
    public string ErrorMessage { get; set; } = "Rate limit exceeded";
    public bool Enabled { get; set; } = true;
}
