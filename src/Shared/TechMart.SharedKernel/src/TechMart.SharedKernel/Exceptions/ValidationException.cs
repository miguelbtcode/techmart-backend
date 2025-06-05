using FluentValidation.Results;
using TechMart.SharedKernel.Common;

namespace TechMart.SharedKernel.Exceptions;

/// <summary>
/// Exception thrown when validation fails.
/// </summary>
public class ValidationException : Exception
{
    /// <summary>
    /// Gets the validation errors.
    /// </summary>
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public ValidationException() : base("One or more validation failures have occurred.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(string message) : base(message)
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(string message, Exception innerException) : base(message, innerException)
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IEnumerable<ValidationFailure> failures) : this()
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
    }

    public ValidationException(IDictionary<string, string[]> errors) : this()
    {
        Errors = errors.ToDictionary(e => e.Key, e => e.Value);
    }

    /// <summary>
    /// Creates a ValidationException with a single error.
    /// </summary>
    /// <param name="propertyName">The property name.</param>
    /// <param name="errorMessage">The error message.</param>
    /// <returns>A ValidationException.</returns>
    public static ValidationException WithError(string propertyName, string errorMessage) =>
        new(new Dictionary<string, string[]> { { propertyName, new[] { errorMessage } } });

    /// <summary>
    /// Creates a ValidationException with multiple errors for a single property.
    /// </summary>
    /// <param name="propertyName">The property name.</param>
    /// <param name="errorMessages">The error messages.</param>
    /// <returns>A ValidationException.</returns>
    public static ValidationException WithErrors(string propertyName, params string[] errorMessages) =>
        new(new Dictionary<string, string[]> { { propertyName, errorMessages } });

    /// <summary>
    /// Converts the exception to a collection of Errors.
    /// </summary>
    /// <returns>A collection of validation errors.</returns>
    public IEnumerable<Error> ToErrors() =>
        Errors.SelectMany(e => e.Value.Select(message => 
            Error.Validation($"Validation.{e.Key}", message)));

    public override string ToString()
    {
        var errorDetails = Errors.SelectMany(e => 
            e.Value.Select(message => $"{e.Key}: {message}"));
        
        return $"{Message} Errors: {string.Join("; ", errorDetails)}";
    }
}