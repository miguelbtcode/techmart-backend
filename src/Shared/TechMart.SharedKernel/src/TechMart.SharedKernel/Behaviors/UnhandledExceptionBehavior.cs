using MediatR;
using Microsoft.Extensions.Logging;
using TechMart.SharedKernel.Common;
using TechMart.SharedKernel.Exceptions;

namespace TechMart.SharedKernel.Behaviors;

/// <summary>
/// Pipeline behavior that handles unhandled exceptions and provides consistent error handling.
/// This should be the outermost behavior in the pipeline.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public class UnhandledExceptionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<UnhandledExceptionBehavior<TRequest, TResponse>> _logger;

    public UnhandledExceptionBehavior(ILogger<UnhandledExceptionBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unhandled exception occurred while processing request {RequestName}: {@Request}",
                requestName,
                request);

            throw;
        }
    }
}

/// <summary>
/// Pipeline behavior that converts unhandled exceptions to Result<T> responses.
/// This prevents exceptions from bubbling up and provides consistent error handling.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response (must be a Result<T>).</typeparam>
public class UnhandledExceptionResultBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : class
{
    private readonly ILogger<UnhandledExceptionResultBehavior<TRequest, TResponse>> _logger;

    public UnhandledExceptionResultBehavior(ILogger<UnhandledExceptionResultBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unhandled exception occurred while processing request {RequestName}: {@Request}",
                requestName,
                request);

            // Try to create an error Result response
            var error = ex switch
            {
                DomainException domainEx => Error.Failure(domainEx.ErrorCode, domainEx.Message),
                NotFoundException notFoundEx => notFoundEx.ToError(),
                ValidationException validationEx => validationEx.ToErrors().First(),
                ConflictException conflictEx => conflictEx.ToError(),
                UnauthorizedException unauthorizedEx => unauthorizedEx.ToError(),
                _ => Error.Failure("UnhandledException", "An unexpected error occurred.")
            };

            var resultType = typeof(TResponse);

            if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(Result<>))
            {
                var valueType = resultType.GetGenericArguments()[0];
                var failureMethod = typeof(Result).GetMethod(nameof(Result.Failure))?.MakeGenericMethod(valueType);
                
                var result = failureMethod?.Invoke(null, [error]);
                return (TResponse)result!;
            }

            if (resultType == typeof(Result))
            {
                return (TResponse)(object)Result.Failure(error);
            }

            // If we can't create a Result, re-throw the exception
            throw;
        }
    }
}