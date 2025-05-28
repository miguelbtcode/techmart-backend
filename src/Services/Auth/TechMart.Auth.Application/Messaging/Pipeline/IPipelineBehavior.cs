namespace TechMart.Auth.Application.Messaging.Pipeline;

/// <summary>
/// Pipeline behavior for cross-cutting concerns
/// </summary>
/// <typeparam name="TRequest">Request type</typeparam>
/// <typeparam name="TResponse">Response type</typeparam>
public interface IPipelineBehavior<in TRequest, TResponse>
    where TRequest : notnull
{
    /// <summary>
    /// Handle the request with the pipeline
    /// </summary>
    Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    );
}

/// <summary>
/// Delegate for the next handler in the pipeline
/// </summary>
public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();
