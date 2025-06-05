using TechMart.SharedKernel.Common;

namespace TechMart.SharedKernel.Exceptions;

/// <summary>
/// Exception thrown when a user is not authenticated or the authentication token is invalid.
/// </summary>
public class UnauthorizedException : Exception
{
    /// <summary>
    /// Gets the reason for the unauthorized access.
    /// </summary>
    public string? Reason { get; }

    /// <summary>
    /// Gets the requested resource that requires authentication.
    /// </summary>
    public string? Resource { get; }

    public UnauthorizedException() : base("Authentication is required to access this resource.")
    {
    }

    public UnauthorizedException(string message) : base(message)
    {
    }

    public UnauthorizedException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public UnauthorizedException(string message, string reason) : base(message)
    {
        Reason = reason;
    }

    public UnauthorizedException(string message, string reason, string resource) : base(message)
    {
        Reason = reason;
        Resource = resource;
    }

    /// <summary>
    /// Creates an UnauthorizedException for an invalid token.
    /// </summary>
    /// <returns>An UnauthorizedException.</returns>
    public static UnauthorizedException InvalidToken() =>
        new("The provided authentication token is invalid or expired.", "InvalidToken");

    /// <summary>
    /// Creates an UnauthorizedException for missing authentication.
    /// </summary>
    /// <returns>An UnauthorizedException.</returns>
    public static UnauthorizedException MissingAuthentication() =>
        new("Authentication credentials are required.", "MissingCredentials");

    /// <summary>
    /// Creates an UnauthorizedException for expired token.
    /// </summary>
    /// <returns>An UnauthorizedException.</returns>
    public static UnauthorizedException TokenExpired() =>
        new("The authentication token has expired.", "TokenExpired");

    /// <summary>
    /// Creates an UnauthorizedException for insufficient permissions.
    /// </summary>
    /// <param name="resource">The resource that was attempted to be accessed.</param>
    /// <returns>An UnauthorizedException.</returns>
    public static UnauthorizedException InsufficientPermissions(string resource) =>
        new($"Insufficient permissions to access '{resource}'.", "InsufficientPermissions", resource);

    /// <summary>
    /// Converts the exception to an Error.
    /// </summary>
    /// <returns>An Error representing the unauthorized access.</returns>
    public Error ToError()
    {
        var code = Reason != null ? $"Auth.{Reason}" : "Auth.Unauthorized";
        return Error.Unauthorized(code, Message);
    }
}