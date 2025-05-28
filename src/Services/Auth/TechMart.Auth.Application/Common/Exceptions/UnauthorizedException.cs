namespace TechMart.Auth.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when user is not authenticated
/// </summary>
public sealed class UnauthorizedException : ApplicationException
{
    public UnauthorizedException()
        : base("User is not authenticated") { }

    public UnauthorizedException(string message)
        : base(message) { }
}
