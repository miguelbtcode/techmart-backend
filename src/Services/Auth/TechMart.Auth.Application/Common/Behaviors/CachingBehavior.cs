using Microsoft.Extensions.Logging;
using TechMart.Auth.Application.Contracts.Infrastructure;
using TechMart.Auth.Application.Messaging.Queries;

namespace TechMart.Auth.Application.Common.Behaviors;

public interface ICacheableQuery
{
    string CacheKey { get; }
    TimeSpan? CacheExpiration { get; }
    bool BypassCache { get; }
}

public sealed class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IQuery<TResponse>, ICacheableQuery
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

    public CachingBehavior(
        ICacheService cacheService,
        ILogger<CachingBehavior<TRequest, TResponse>> logger
    )
    {
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        if (request.BypassCache)
        {
            _logger.LogDebug("Cache bypassed for {RequestType}", typeof(TRequest).Name);
            return await next();
        }

        var cacheKey = request.CacheKey;

        try
        {
            var cachedResponse = await _cacheService.GetAsync<TResponse>(
                cacheKey,
                cancellationToken
            );
            if (cachedResponse != null)
            {
                _logger.LogDebug("Cache hit for key {CacheKey}", cacheKey);
                return cachedResponse;
            }

            _logger.LogDebug("Cache miss for key {CacheKey}", cacheKey);
            var response = await next();

            if (response != null)
            {
                await _cacheService.SetAsync(
                    cacheKey,
                    response,
                    request.CacheExpiration,
                    cancellationToken
                );

                _logger.LogDebug(
                    "Response cached with key {CacheKey} for {ExpirationTime}",
                    cacheKey,
                    request.CacheExpiration
                );
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Cache operation failed for key {CacheKey}, proceeding without cache",
                cacheKey
            );
            return await next();
        }
    }
}
