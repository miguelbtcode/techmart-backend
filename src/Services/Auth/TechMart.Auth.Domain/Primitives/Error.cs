namespace TechMart.Auth.Domain.Primitives;

/// <summary>
/// Represents an error in the domain with code and message
/// </summary>
public sealed class Error : IEquatable<Error>
{
    /// <summary>
    /// Represents a successful operation (no error)
    /// </summary>
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.None);
    public static readonly Error NullValue = new(
        "Error.NullValue",
        "Un valor Null fue ingresado",
        ErrorType.None
    );

    /// <summary>
    /// Creates a new error instance
    /// </summary>
    /// <param name="code">Unique error code for programmatic handling</param>
    /// <param name="message">Human-readable error message</param>
    /// <param name="type">Type of error for categorization</param>
    private Error(string code, string message, ErrorType type)
    {
        Code = code;
        Message = message;
        Type = type;
    }

    /// <summary>
    /// Unique error code for programmatic handling
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Human-readable error message
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Type of error for categorization
    /// </summary>
    public ErrorType Type { get; }

    /// <summary>
    /// Creates a failure error
    /// </summary>
    public static Error Failure(string code, string message) =>
        new(code, message, ErrorType.Failure);

    /// <summary>
    /// Creates a validation error
    /// </summary>
    public static Error Validation(string code, string message) =>
        new(code, message, ErrorType.Validation);

    /// <summary>
    /// Creates a not found error
    /// </summary>
    public static Error NotFound(string code, string message) =>
        new(code, message, ErrorType.NotFound);

    /// <summary>
    /// Creates a conflict error
    /// </summary>
    public static Error Conflict(string code, string message) =>
        new(code, message, ErrorType.Conflict);

    /// <summary>
    /// Creates an unauthorized error
    /// </summary>
    public static Error Unauthorized(string code, string message) =>
        new(code, message, ErrorType.Unauthorized);

    /// <summary>
    /// Creates a forbidden error
    /// </summary>
    public static Error Forbidden(string code, string message) =>
        new(code, message, ErrorType.Forbidden);

    /// <summary>
    /// Implicit conversion to string returns the error code
    /// </summary>
    public static implicit operator string(Error error) => error.Code;

    /// <summary>
    /// Implicit conversion from string creates a failure error
    /// </summary>
    public static implicit operator Error(string code) => Failure(code, code);

    #region Equality Implementation

    public bool Equals(Error? other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return Code == other.Code && Message == other.Message && Type == other.Type;
    }

    public override bool Equals(object? obj) => Equals(obj as Error);

    public override int GetHashCode() => HashCode.Combine(Code, Message, Type);

    public static bool operator ==(Error? left, Error? right) => Equals(left, right);

    public static bool operator !=(Error? left, Error? right) => !Equals(left, right);

    #endregion

    public override string ToString() => $"[{Type}:{Code}] {Message}";
}

/// <summary>
/// Types of errors for categorization
/// </summary>
public enum ErrorType
{
    None = 0,
    Failure = 1,
    Validation = 2,
    NotFound = 3,
    Conflict = 4,
    Unauthorized = 5,
    Forbidden = 6,
}
