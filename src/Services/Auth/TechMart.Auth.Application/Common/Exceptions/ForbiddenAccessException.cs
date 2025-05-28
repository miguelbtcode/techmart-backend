namespace TechMart.Auth.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when user access is forbidden
/// </summary>
public sealed class ForbiddenAccessException(string message) : ApplicationException(message) { }
