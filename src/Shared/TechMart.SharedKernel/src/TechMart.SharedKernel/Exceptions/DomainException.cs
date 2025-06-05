using TechMart.SharedKernel.Common;

namespace TechMart.SharedKernel.Exceptions;

/// <summary>
/// Represents errors that occur in the domain layer.
/// These are business rule violations and domain invariant failures.
/// </summary>
public class DomainException : Exception
{
    /// <summary>
    /// Gets the error code associated with the exception.
    /// </summary>
    public string ErrorCode { get; }

    /// <summary>
    /// Gets additional details about the error.
    /// </summary>
    public IDictionary<string, object> Details { get; }

    public DomainException(string message) : base(message)
    {
        ErrorCode = "Domain.Error";
        Details = new Dictionary<string, object>();
    }

    public DomainException(string errorCode, string message) : base(message)
    {
        ErrorCode = errorCode;
        Details = new Dictionary<string, object>();
    }

    public DomainException(string errorCode, string message, Exception innerException) : base(message, innerException)
    {
        ErrorCode = errorCode;
        Details = new Dictionary<string, object>();
    }

    public DomainException(string errorCode, string message, IDictionary<string, object> details) : base(message)
    {
        ErrorCode = errorCode;
        Details = details ?? new Dictionary<string, object>();
    }

    /// <summary>
    /// Creates a domain exception from an Error.
    /// </summary>
    /// <param name="error">The error.</param>
    /// <returns>A domain exception.</returns>
    public static DomainException FromError(Error error) =>
        new(error.Code, error.Message);

    /// <summary>
    /// Adds additional details to the exception.
    /// </summary>
    /// <param name="key">The detail key.</param>
    /// <param name="value">The detail value.</param>
    /// <returns>The current exception for method chaining.</returns>
    public DomainException WithDetail(string key, object value)
    {
        Details[key] = value;
        return this;
    }

    public override string ToString()
    {
        var details = Details.Any() 
            ? $" Details: {string.Join(", ", Details.Select(d => $"{d.Key}={d.Value}"))}"
            : string.Empty;
        
        return $"[{ErrorCode}] {Message}{details}";
    }
}