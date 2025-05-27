using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TechMart.Auth.Application.Abstractions.Caching;
using TechMart.Auth.Application.Abstractions.Contracts;
using TechMart.Auth.Domain.Users.Entities;
using TechMart.Auth.Domain.Users.ValueObjects;
using TechMart.Auth.Infrastructure.Authentication.Models;
using TechMart.Auth.Infrastructure.Caching;
using TechMart.Auth.Infrastructure.Settings;
using TokenValidationResult = TechMart.Auth.Application.Abstractions.Authentication.TokenValidationResult;

namespace TechMart.Auth.Infrastructure.Authentication;

public class RedisTokenService
    : IRefreshTokenService,
        IPasswordResetService,
        IEmailConfirmationService
{
    private readonly ICacheService _cache;
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<RedisTokenService> _logger;

    public RedisTokenService(
        ICacheService cache,
        IOptions<JwtSettings> jwtSettings,
        ILogger<RedisTokenService> logger
    )
    {
        _cache = cache;
        _jwtSettings = jwtSettings.Value;
        _logger = logger;
    }

    #region Refresh Tokens

    public async Task<string> GenerateRefreshTokenAsync(User user)
    {
        try
        {
            var refreshToken = GenerateSecureToken();
            var tokenHash = ComputeTokenHash(refreshToken);

            // Store token data with user ID as key
            var key = CacheKeys.RefreshToken(user.Id);
            await _cache.SetAsync(
                key,
                new RefreshTokenData
                {
                    UserId = user.Id,
                    Email = user.Email.Value,
                    TokenHash = tokenHash,
                    CreatedAt = DateTime.UtcNow,
                },
                TimeSpan.FromDays(_jwtSettings.RefreshTokenExpirationDays)
            );

            // Store reverse lookup: token hash -> user ID (for validation)
            var reverseKey = CacheKeys.RefreshTokenReverse(tokenHash);
            await _cache.SetAsync(
                reverseKey,
                user.Id,
                TimeSpan.FromDays(_jwtSettings.RefreshTokenExpirationDays)
            );

            _logger.LogDebug("Refresh token generated for user {UserId}", user.Id);
            return refreshToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate refresh token for user {UserId}", user.Id);
            throw;
        }
    }

    public async Task<TokenValidationResult> ValidateRefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return new TokenValidationResult(false, null, null);

            var tokenHash = ComputeTokenHash(refreshToken);

            // Find user ID using reverse lookup
            var reverseKey = CacheKeys.RefreshTokenReverse(tokenHash);
            var userId = await _cache.GetAsync<Guid>(reverseKey, cancellationToken);

            if (userId == Guid.Empty)
            {
                _logger.LogWarning("Refresh token not found or expired");
                return new TokenValidationResult(false, null, null);
            }

            // Verify token data
            var key = CacheKeys.RefreshToken(userId);
            var tokenData = await _cache.GetAsync<RefreshTokenData>(key, cancellationToken);

            if (tokenData?.TokenHash != tokenHash)
            {
                _logger.LogWarning("Refresh token hash mismatch for user {UserId}", userId);
                return new TokenValidationResult(false, null, null);
            }

            _logger.LogDebug("Refresh token validated successfully for user {UserId}", userId);
            return new TokenValidationResult(
                IsValid: true,
                UserId: userId,
                ExpiresAt: tokenData.CreatedAt.AddDays(_jwtSettings.RefreshTokenExpirationDays)
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate refresh token");
            return new TokenValidationResult(false, null, null);
        }
    }

    public async Task RevokeRefreshTokenAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            // Get current token data to remove reverse lookup
            var key = CacheKeys.RefreshToken(userId);
            var tokenData = await _cache.GetAsync<RefreshTokenData>(key, cancellationToken);

            if (tokenData != null)
            {
                // Remove reverse lookup
                var reverseKey = CacheKeys.RefreshTokenReverse(tokenData.TokenHash);
                await _cache.RemoveAsync(reverseKey, cancellationToken);
            }

            // Remove main token data
            await _cache.RemoveAsync(key, cancellationToken);

            _logger.LogDebug("Refresh token revoked for user {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to revoke refresh token for user {UserId}", userId);
            throw;
        }
    }

    public async Task RevokeAllRefreshTokensAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            // For now, we only support one refresh token per user
            // In the future, we could store multiple tokens with device identifiers
            await RevokeRefreshTokenAsync(userId, cancellationToken);

            _logger.LogDebug("All refresh tokens revoked for user {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to revoke all refresh tokens for user {UserId}", userId);
            throw;
        }
    }

    #endregion

    #region Password Reset Tokens

    public async Task<string> GenerateResetTokenAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var token = GenerateSecureToken();
            var tokenHash = ComputeTokenHash(token);

            // Use userId as the key for password reset tokens
            var key = CacheKeys.PasswordResetToken(userId.Value.ToString());
            await _cache.SetAsync(
                key,
                new PasswordResetTokenData
                {
                    UserId = userId,
                    TokenHash = tokenHash,
                    CreatedAt = DateTime.UtcNow,
                },
                TimeSpan.FromHours(1),
                cancellationToken
            );

            _logger.LogDebug("Password reset token generated for user {UserId}", userId);
            return token;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to generate password reset token for user {UserId}",
                userId
            );
            throw;
        }
    }

    public async Task<UserId?> ValidateResetTokenAsync(
        string email,
        string token,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
                return null;

            // We need to find the user first, then check their reset token
            // This is a limitation of our current key structure
            var tokenHash = ComputeTokenHash(token);

            // Search pattern for password reset tokens
            var pattern = CacheKeys.PasswordResetPattern();
            var keys = await _cache.GetKeysByPatternAsync(pattern, cancellationToken);

            foreach (var key in keys)
            {
                var tokenData = await _cache.GetAsync<PasswordResetTokenData>(
                    key,
                    cancellationToken
                );
                if (tokenData?.TokenHash == tokenHash)
                {
                    _logger.LogDebug(
                        "Password reset token validated for user {UserId}",
                        tokenData.UserId
                    );
                    return tokenData.UserId;
                }
            }

            _logger.LogWarning(
                "Password reset token not found or expired for email {Email}",
                email
            );
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to validate password reset token for email {Email}",
                email
            );
            return null;
        }
    }

    public async Task InvalidateTokenAsync(
        string token,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var tokenHash = ComputeTokenHash(token);

            // Search for the token across all password reset keys
            var pattern = CacheKeys.PasswordResetPattern();
            var keys = await _cache.GetKeysByPatternAsync(pattern, cancellationToken);

            foreach (var key in keys)
            {
                var tokenData = await _cache.GetAsync<PasswordResetTokenData>(
                    key,
                    cancellationToken
                );
                if (tokenData?.TokenHash == tokenHash)
                {
                    await _cache.RemoveAsync(key, cancellationToken);
                    _logger.LogDebug("Password reset token invalidated");
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to invalidate password reset token");
            throw;
        }
    }

    #endregion

    #region Email Confirmation Tokens

    public async Task<string> GenerateTokenAsync(
        string email,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var token = GenerateSecureToken();
            var tokenHash = ComputeTokenHash(token);

            var key = CacheKeys.EmailConfirmationToken(email);
            await _cache.SetAsync(
                key,
                new EmailConfirmationTokenData
                {
                    Email = email,
                    TokenHash = tokenHash,
                    CreatedAt = DateTime.UtcNow,
                },
                TimeSpan.FromDays(1),
                cancellationToken
            );

            _logger.LogDebug("Email confirmation token generated for email {Email}", email);
            return token;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to generate email confirmation token for email {Email}",
                email
            );
            throw;
        }
    }

    public async Task<bool> ValidateTokenAsync(
        string email,
        string token,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
                return false;

            var tokenHash = ComputeTokenHash(token);
            var key = CacheKeys.EmailConfirmationToken(email);
            var tokenData = await _cache.GetAsync<EmailConfirmationTokenData>(
                key,
                cancellationToken
            );

            var isValid = tokenData?.TokenHash == tokenHash;

            if (isValid)
            {
                // Remove token after successful validation (one-time use)
                await _cache.RemoveAsync(key, cancellationToken);
                _logger.LogDebug(
                    "Email confirmation token validated and removed for email {Email}",
                    email
                );
            }
            else
            {
                _logger.LogWarning(
                    "Email confirmation token validation failed for email {Email}",
                    email
                );
            }

            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to validate email confirmation token for email {Email}",
                email
            );
            return false;
        }
    }

    #endregion

    #region Helper Methods

    private string GenerateSecureToken()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[32];
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes).Replace('+', '-').Replace('/', '_').TrimEnd('='); // URL-safe base64
    }

    private string ComputeTokenHash(string token)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(bytes);
    }

    #endregion
}
