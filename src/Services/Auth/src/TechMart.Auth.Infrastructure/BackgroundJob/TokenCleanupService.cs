using TechMart.Auth.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TechMart.Auth.Infrastructure.BackgroundJob;

public class TokenCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TokenCleanupService> _logger;
    private readonly TimeSpan _period = TimeSpan.FromHours(1); // Run every hour

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
        _logger.LogInformation("Token Cleanup Service started");

        using var timer = new PeriodicTimer(_period);

        while (
            !stoppingToken.IsCancellationRequested
            && await timer.WaitForNextTickAsync(stoppingToken)
        )
        {
            await CleanupExpiredTokensAsync();
        }

        _logger.LogInformation("Token Cleanup Service stopped");
    }

    private async Task CleanupExpiredTokensAsync()
    {
        try
        {
            _logger.LogInformation("Starting token cleanup...");

            using var scope = _serviceProvider.CreateScope();

            var refreshTokenRepo =
                scope.ServiceProvider.GetRequiredService<IRefreshTokenRepository>();
            var emailTokenRepo =
                scope.ServiceProvider.GetRequiredService<IEmailVerificationTokenRepository>();
            var passwordResetRepo =
                scope.ServiceProvider.GetRequiredService<IPasswordResetTokenRepository>();

            // Cleanup expired tokens
            await refreshTokenRepo.CleanupExpiredTokensAsync();
            await emailTokenRepo.CleanupExpiredTokensAsync();
            await passwordResetRepo.CleanupExpiredTokensAsync();

            _logger.LogInformation("Token cleanup completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during token cleanup");
        }
    }
}
