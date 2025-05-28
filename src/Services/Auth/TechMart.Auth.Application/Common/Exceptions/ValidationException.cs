using FluentValidation.Results;

namespace TechMart.Auth.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when validation fails
/// </summary>
public sealed class ValidationException : ApplicationException
{
    public ValidationException()
        : base("One or more validation failures have occurred")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
    }

    public ValidationException(string propertyName, string errorMessage)
        : this()
    {
        Errors = new Dictionary<string, string[]> { { propertyName, new[] { errorMessage } } };
    }

    public IDictionary<string, string[]> Errors { get; }
}
