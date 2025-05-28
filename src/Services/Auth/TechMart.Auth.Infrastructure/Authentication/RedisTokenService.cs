using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TechMart.Auth.Application.Contracts.Authentication;
using TechMart.Auth.Application.Contracts.Infrastructure;
using TechMart.Auth.Domain.Users.Entities;
using TechMart.Auth.Domain.Users.ValueObjects;
using TechMart.Auth.Infrastructure.Authentication.Models;
using TechMart.Auth.Infrastructure.Caching;
using TechMart.Auth.Infrastructure.Settings;

namespace TechMart.Auth.Infrastructure.Authentication;

/// <summary>
/// Redis-based implementation of token services
/// Handles refresh tokens, password reset tokens, and email confirmation tokens
/// </summary>
public sealed class RedisTokenService
    : IRefreshTokenService,
        IPasswordResetService,
        IEmailConfirmationService
{
    private readonly ICacheService _cache;
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<RedisTokenService> _logger;

    // Token expiration constants
    private static readonly TimeSpan EmailConfirmationExpiry = TimeSpan.FromDays(1);
    private static readonly TimeSpan PasswordResetExpiry = TimeSpan.FromHours(1);

    public RedisTokenService(
        ICacheService cache,
        IOptions<JwtSettings> jwtSettings,
        ILogger<RedisTokenService> logger
    )
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _jwtSettings = jwtSettings?.Value ?? throw new ArgumentNullException(nameof(jwtSettings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    #region Refresh Tokens

    public async Task<string> GenerateRefreshTokenAsync(
        User user,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(user);

        try
        {
            var refreshToken = GenerateSecureToken();
            var tokenHash = ComputeTokenHash(refreshToken);

            // Store token data with user ID as key
            var key = CacheKeys.RefreshToken(user.Id.Value);
            var tokenData = new RefreshTokenData
            {
                UserId = user.Id.Value,
                Email = user.Email.Value,
                TokenHash = tokenHash,
                CreatedAt = DateTime.UtcNow,
            };

            var expiry = TimeSpan.FromDays(_jwtSettings.RefreshTokenExpirationDays);
            await _cache.SetAsync(key, tokenData, expiry, cancellationToken);

            // Store reverse lookup: token hash -> user ID (for validation)
            var reverseKey = CacheKeys.RefreshTokenReverse(tokenHash);
            await _cache.SetAsync(reverseKey, user.Id.Value, expiry, cancellationToken);

            _logger.LogDebug("Refresh token generated for user {UserId}", user.Id.Value);
            return refreshToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to generate refresh token for user {UserId}",
                user.Id.Value
            );
            throw new InvalidOperationException("Failed to generate refresh token", ex);
        }
    }

    public async Task<TokenValidationResult> ValidateRefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return new TokenValidationResult(
                false,
                null,
                null,
                null,
                "Refresh token is null or empty"
            );
        }

        try
        {
            var tokenHash = ComputeTokenHash(refreshToken);

            // Find user ID using reverse lookup
            var reverseKey = CacheKeys.RefreshTokenReverse(tokenHash);
            var userId = await _cache.GetAsync<Guid>(reverseKey, cancellationToken);

            if (userId == Guid.Empty)
            {
                _logger.LogWarning("Refresh token not found or expired");
                return new TokenValidationResult(
                    false,
                    null,
                    null,
                    null,
                    "Token not found or expired"
                );
            }

            // Verify token data
            var key = CacheKeys.RefreshToken(userId);
            var tokenData = await _cache.GetAsync<RefreshTokenData>(key, cancellationToken);

            if (tokenData?.TokenHash != tokenHash)
            {
                _logger.LogWarning("Refresh token hash mismatch for user {UserId}", userId);
                return new TokenValidationResult(false, null, null, null, "Invalid token");
            }

            var expiresAt = tokenData.CreatedAt.AddDays(_jwtSettings.RefreshTokenExpirationDays);

            _logger.LogDebug("Refresh token validated successfully for user {UserId}", userId);
            return new TokenValidationResult(IsValid: true, UserId: userId, ExpiresAt: expiresAt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate refresh token");
            return new TokenValidationResult(false, null, null, null, "Token validation failed");
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
            throw new InvalidOperationException("Failed to revoke refresh token", ex);
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
            throw new InvalidOperationException("Failed to revoke all refresh tokens", ex);
        }
    }

    public async Task<IEnumerable<RefreshTokenInfo>> GetActiveTokensAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var key = CacheKeys.RefreshToken(userId);
            var tokenData = await _cache.GetAsync<RefreshTokenData>(key, cancellationToken);

            if (tokenData == null)
            {
                return Enumerable.Empty<RefreshTokenInfo>();
            }

            var tokenInfo = new RefreshTokenInfo(
                TokenId: tokenData.TokenHash[..8], // First 8 chars for identification
                UserId: tokenData.UserId,
                CreatedAt: tokenData.CreatedAt,
                ExpiresAt: tokenData.CreatedAt.AddDays(_jwtSettings.RefreshTokenExpirationDays)
            );

            return new[] { tokenInfo };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get active tokens for user {UserId}", userId);
            return Enumerable.Empty<RefreshTokenInfo>();
        }
    }

    public async Task CleanupExpiredTokensAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // This would be implemented with a background service
            // For now, Redis handles expiration automatically
            _logger.LogDebug("Token cleanup completed (handled by Redis TTL)");
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cleanup expired tokens");
        }
    }

    #endregion

    #region Password Reset Tokens

    public async Task<string> GenerateResetTokenAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(userId);

        try
        {
            var token = GenerateSecureToken();
            var tokenHash = ComputeTokenHash(token);

            var key = CacheKeys.PasswordResetToken(userId.Value.ToString());
            var tokenData = new PasswordResetTokenData
            {
                UserId = userId,
                TokenHash = tokenHash,
                CreatedAt = DateTime.UtcNow,
            };

            await _cache.SetAsync(key, tokenData, PasswordResetExpiry, cancellationToken);

            _logger.LogDebug("Password reset token generated for user {UserId}", userId.Value);
            return token;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to generate password reset token for user {UserId}",
                userId.Value
            );
            throw new InvalidOperationException("Failed to generate password reset token", ex);
        }
    }

    public async Task<UserId?> ValidateResetTokenAsync(
        string email,
        string token,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        try
        {
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
                        tokenData.UserId.Value
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
        if (string.IsNullOrWhiteSpace(token))
        {
            return;
        }

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
            throw new InvalidOperationException("Failed to invalidate password reset token", ex);
        }
    }

    public async Task InvalidateAllTokensAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var key = CacheKeys.PasswordResetToken(userId.Value.ToString());
            await _cache.RemoveAsync(key, cancellationToken);

            _logger.LogDebug(
                "All password reset tokens invalidated for user {UserId}",
                userId.Value
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to invalidate all tokens for user {UserId}", userId.Value);
            throw new InvalidOperationException("Failed to invalidate all tokens", ex);
        }
    }

    public async Task<bool> IsPasswordResetTokenValidAsync(
        string email,
        string token,
        CancellationToken cancellationToken = default
    )
    {
        var userId = await ValidateResetTokenAsync(email, token, cancellationToken);
        return userId != null;
    }

    public async Task<DateTime?> GetPasswordResetTokenExpirationAsync(
        string token,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        try
        {
            var tokenHash = ComputeTokenHash(token);

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
                    return tokenData.CreatedAt.Add(PasswordResetExpiry);
                }
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get token expiration for token");
            return null;
        }
    }

    #endregion

    #region Email Confirmation Tokens

    public async Task<string> GenerateTokenAsync(
        string email,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email cannot be null or empty", nameof(email));
        }

        try
        {
            var token = GenerateSecureToken();
            var tokenHash = ComputeTokenHash(token);

            var key = CacheKeys.EmailConfirmationToken(email);
            var tokenData = new EmailConfirmationTokenData
            {
                Email = email.ToLowerInvariant(),
                TokenHash = tokenHash,
                CreatedAt = DateTime.UtcNow,
            };

            await _cache.SetAsync(key, tokenData, EmailConfirmationExpiry, cancellationToken);

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
            throw new InvalidOperationException("Failed to generate email confirmation token", ex);
        }
    }

    public async Task<bool> ValidateTokenAsync(
        string email,
        string token,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        try
        {
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

    public async Task InvalidateTokensAsync(
        string email,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return;
        }

        try
        {
            var key = CacheKeys.EmailConfirmationToken(email);
            await _cache.RemoveAsync(key, cancellationToken);

            _logger.LogDebug("Email confirmation tokens invalidated for email {Email}", email);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to invalidate email confirmation tokens for email {Email}",
                email
            );
            throw new InvalidOperationException(
                "Failed to invalidate email confirmation tokens",
                ex
            );
        }
    }

    public async Task<bool> IsEmailConfirmationTokenValidAsync(
        string email,
        string token,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        try
        {
            var tokenHash = ComputeTokenHash(token);
            var key = CacheKeys.EmailConfirmationToken(email);
            var tokenData = await _cache.GetAsync<EmailConfirmationTokenData>(
                key,
                cancellationToken
            );

            return tokenData?.TokenHash == tokenHash;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to check if email confirmation token is valid for email {Email}",
                email
            );
            return false;
        }
    }

    public async Task<DateTime?> GetEmailConfirmationTokenExpirationAsync(
        string email,
        string token,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        try
        {
            var key = CacheKeys.EmailConfirmationToken(email);
            var tokenData = await _cache.GetAsync<EmailConfirmationTokenData>(
                key,
                cancellationToken
            );

            if (tokenData != null && tokenData.TokenHash == ComputeTokenHash(token))
            {
                return tokenData.CreatedAt.Add(EmailConfirmationExpiry);
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to get email confirmation token expiration for email {Email}",
                email
            );
            return null;
        }
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Generates a cryptographically secure random token
    /// </summary>
    private static string GenerateSecureToken()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[32]; // 256 bits
        rng.GetBytes(bytes);

        // URL-safe base64 encoding
        return Convert.ToBase64String(bytes).Replace('+', '-').Replace('/', '_').TrimEnd('=');
    }

    /// <summary>
    /// Computes SHA256 hash of a token for secure storage
    /// </summary>
    private static string ComputeTokenHash(string token)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(bytes);
    }

    #endregion
}
