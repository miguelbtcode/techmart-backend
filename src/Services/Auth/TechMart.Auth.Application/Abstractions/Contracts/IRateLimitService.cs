namespace TechMart.Auth.Application.Abstractions.Contracts;

public interface IRateLimitService
{
    Task<bool> IsAllowedAsync(string identifier, int maxAttempts = 5, TimeSpan? window = null);
    Task IncrementAsync(string identifier, TimeSpan? window = null);
    Task ResetAsync(string identifier);
}
