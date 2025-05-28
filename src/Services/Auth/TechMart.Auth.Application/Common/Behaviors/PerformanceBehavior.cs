using System.Diagnostics;
using Microsoft.Extensions.Logging;
using TechMart.Auth.Application.Contracts.Authentication;

namespace TechMart.Auth.Application.Common.Behaviors;

public sealed class PerformanceBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly Stopwatch _timer;

    public PerformanceBehavior(
        ILogger<PerformanceBehavior<TRequest, TResponse>> logger,
        ICurrentUserService currentUserService
    )
    {
        _logger = logger;
        _currentUserService = currentUserService;
        _timer = new Stopwatch();
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        _timer.Start();

        var response = await next();

        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        // Log warning for long-running requests (> 500ms)
        if (elapsedMilliseconds > 500)
        {
            var requestName = typeof(TRequest).Name;
            var userId = await _currentUserService.GetCurrentUserIdAsync(cancellationToken);

            _logger.LogWarning(
                "Long Running Request: {RequestName} ({ElapsedMs} ms) for user {UserId} with data {@Request}",
                requestName,
                elapsedMilliseconds,
                userId,
                request
            );
        }

        return response;
    }
}
