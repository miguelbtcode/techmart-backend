namespace TechMart.SharedKernel.Common;

/// <summary>
/// Represents an error with a code and message.
/// </summary>
public sealed record Error(string Code, string Message, ErrorType Type = ErrorType.Failure)
{
    /// <summary>
    /// Represents no error (successful operation).
    /// </summary>
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.None);

    /// <summary>
    /// Represents a null value error.
    /// </summary>
    public static readonly Error NullValue = new("Error.NullValue", "The specified result value is null.", ErrorType.Failure);

    /// <summary>
    /// Gets a value indicating whether this is a validation error.
    /// </summary>
    public bool IsValidation => Type == ErrorType.Validation;

    /// <summary>
    /// Gets a value indicating whether this is a not found error.
    /// </summary>
    public bool IsNotFound => Type == ErrorType.NotFound;

    /// <summary>
    /// Gets a value indicating whether this is a conflict error.
    /// </summary>
    public bool IsConflict => Type == ErrorType.Conflict;

    /// <summary>
    /// Gets a value indicating whether this is an unauthorized error.
    /// </summary>
    public bool IsUnauthorized => Type == ErrorType.Unauthorized;

    /// <summary>
    /// Gets a value indicating whether this is a forbidden error.
    /// </summary>
    public bool IsForbidden => Type == ErrorType.Forbidden;

    /// <summary>
    /// Creates a failure error.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A failure error.</returns>
    public static Error Failure(string code, string message) =>
        new(code, message, ErrorType.Failure);

    /// <summary>
    /// Creates a validation error.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A validation error.</returns>
    public static Error Validation(string code, string message) =>
        new(code, message, ErrorType.Validation);

    /// <summary>
    /// Creates a not found error.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A not found error.</returns>
    public static Error NotFound(string code, string message) =>
        new(code, message, ErrorType.NotFound);

    /// <summary>
    /// Creates a conflict error.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A conflict error.</returns>
    public static Error Conflict(string code, string message) =>
        new(code, message, ErrorType.Conflict);

    /// <summary>
    /// Creates an unauthorized error.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    /// <returns>An unauthorized error.</returns>
    public static Error Unauthorized(string code, string message) =>
        new(code, message, ErrorType.Unauthorized);

    /// <summary>
    /// Creates a forbidden error.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A forbidden error.</returns>
    public static Error Forbidden(string code, string message) =>
        new(code, message, ErrorType.Forbidden);

    /// <summary>
    /// Implicitly converts an error to its code.
    /// </summary>
    /// <param name="error">The error.</param>
    /// <returns>The error code.</returns>
    public static implicit operator string(Error error) => error.Code;

    /// <summary>
    /// Implicitly converts an error to a Result.
    /// </summary>
    /// <param name="error">The error.</param>
    /// <returns>A failed result.</returns>
    public static implicit operator Result(Error error) => Result.Failure(error);

    public override string ToString() => $"{Code}: {Message}";
}

/// <summary>
/// Represents the type of an error.
/// </summary>
public enum ErrorType
{
    None = 0,
    Failure = 1,
    Validation = 2,
    NotFound = 3,
    Conflict = 4,
    Unauthorized = 5,
    Forbidden = 6
}