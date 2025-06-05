using FluentValidation;
using MediatR;
using TechMart.SharedKernel.Common;

namespace TechMart.SharedKernel.Behaviors;

/// <summary>
/// Pipeline behavior that validates requests using FluentValidation.
/// This behavior runs before the request handler and validates the request object.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .Where(r => r.Errors.Any())
            .SelectMany(r => r.Errors)
            .ToArray();

        if (failures.Any())
        {
            throw new ValidationException(failures);
        }

        return await next();
    }
}

/// <summary>
/// Pipeline behavior that validates requests and returns validation errors as Result<T>.
/// This is an alternative to ValidationBehavior that doesn't throw exceptions.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response (must be a Result<T>).</typeparam>
public class ValidationResultBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : class
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationResultBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .Where(r => r.Errors.Any())
            .SelectMany(r => r.Errors)
            .ToArray();

        if (failures.Any())
        {
            // Try to create a Result.Failure response
            var resultType = typeof(TResponse);
            
            if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(Result<>))
            {
                var valueType = resultType.GetGenericArguments()[0];
                var failureMethod = typeof(Result).GetMethod(nameof(Result.Failure))?.MakeGenericMethod(valueType);
                
                var firstFailure = failures.First();
                var error = Error.Validation($"Validation.{firstFailure.PropertyName}", firstFailure.ErrorMessage);
                
                var result = failureMethod?.Invoke(null, new object[] { error });
                return (TResponse)result!;
            }
            
            if (resultType == typeof(Result))
            {
                var firstFailure = failures.First();
                var error = Error.Validation($"Validation.{firstFailure.PropertyName}", firstFailure.ErrorMessage);
                return (TResponse)(object)Result.Failure(error);
            }
        }

        return await next();
    }
}