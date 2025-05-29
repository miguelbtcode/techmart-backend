using AuthMicroservice.Repositories;

namespace AuthMicroservice.Services;

public class TokenCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TokenCleanupService> _logger;

    public TokenCleanupService(
        IServiceProvider serviceProvider,
        ILogger<TokenCleanupService> logger
    )
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var refreshTokenRepository =
                    scope.ServiceProvider.GetRequiredService<IRefreshTokenRepository>();

                await refreshTokenRepository.CleanupExpiredTokensAsync();
                _logger.LogInformation("Tokens expirados limpiados a las {Time}", DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error limpiando tokens expirados");
            }

            // Ejecutar cada 24 horas
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}
