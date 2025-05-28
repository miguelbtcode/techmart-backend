using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TechMart.Auth.Application.Messaging.Pipeline;

/// <summary>
/// Request pipeline coordinator
/// </summary>
public sealed class RequestPipeline
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RequestPipeline> _logger;

    public RequestPipeline(IServiceProvider serviceProvider, ILogger<RequestPipeline> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// Execute request through the pipeline
    /// </summary>
    public async Task<TResponse> ExecuteAsync<TRequest, TResponse>(
        TRequest request,
        Func<TRequest, CancellationToken, Task<TResponse>> handler,
        CancellationToken cancellationToken = default
    )
        where TRequest : notnull
    {
        var behaviors = _serviceProvider
            .GetServices<IPipelineBehavior<TRequest, TResponse>>()
            .ToList();

        if (!behaviors.Any())
        {
            _logger.LogDebug(
                "No pipeline behaviors found for {RequestType}",
                typeof(TRequest).Name
            );
            return await handler(request, cancellationToken);
        }

        _logger.LogDebug(
            "Executing {BehaviorCount} pipeline behaviors for {RequestType}",
            behaviors.Count,
            typeof(TRequest).Name
        );

        // Build the pipeline from the inside out
        RequestHandlerDelegate<TResponse> pipeline = () => handler(request, cancellationToken);

        // Apply behaviors in reverse order so they execute in the correct order
        for (int i = behaviors.Count - 1; i >= 0; i--)
        {
            var behavior = behaviors[i];
            var currentPipeline = pipeline;
            pipeline = () => behavior.Handle(request, currentPipeline, cancellationToken);
        }

        return await pipeline();
    }
}
