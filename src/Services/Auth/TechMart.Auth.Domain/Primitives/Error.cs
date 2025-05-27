namespace TechMart.Auth.Domain.Primitives;

/// <summary>
/// Represents an error in the domain with code and message
/// </summary>
public record Error : IEquatable<Error>
{
    /// <summary>
    /// Represents a successful operation (no error)
    /// </summary>
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.None);
    public static readonly Error NullValue = new(
        "Error.NullValue",
        "Un valor Null fue ingresado",
        ErrorType.Failure
    );

    public Error(string code, string message, ErrorType type)
    {
        Code = code;
        Message = message;
        Type = type;
    }

    public string Code { get; }

    public string Message { get; }

    public ErrorType Type { get; }

    public static Error Failure(string code, string message) =>
        new(code, message, ErrorType.Failure);

    public static Error Validation(string code, string message) =>
        new(code, message, ErrorType.Validation);

    public static Error NotFound(string code, string message) =>
        new(code, message, ErrorType.NotFound);

    public static Error Problem(string code, string description) =>
        new(code, description, ErrorType.Problem);

    public static Error Conflict(string code, string message) =>
        new(code, message, ErrorType.Conflict);

    public static Error Unauthorized(string code, string message) =>
        new(code, message, ErrorType.Unauthorized);

    public static Error Forbidden(string code, string message) =>
        new(code, message, ErrorType.Forbidden);

    public static implicit operator string(Error error) => error.Code;

    public static implicit operator Error(string code) => Failure(code, code);
}

/// <summary>
/// Types of errors for categorization
/// </summary>
public enum ErrorType
{
    None = 0,
    Failure = 1,
    Validation = 2,
    Problem = 3,
    NotFound = 4,
    Conflict = 5,
    Unauthorized = 6,
    Forbidden = 7,
}
