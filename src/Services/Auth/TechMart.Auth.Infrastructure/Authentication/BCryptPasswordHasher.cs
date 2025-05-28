using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TechMart.Auth.Application.Contracts.Authentication;
using TechMart.Auth.Infrastructure.Settings;

namespace TechMart.Auth.Infrastructure.Authentication;

/// <summary>
/// BCrypt implementation of password hashing service
/// Provides secure password hashing and verification using BCrypt algorithm
/// </summary>
public sealed class BCryptPasswordHasher : IPasswordHasher
{
    private readonly BCryptSettings _settings;
    private readonly ILogger<BCryptPasswordHasher> _logger;

    public BCryptPasswordHasher(
        IOptions<BCryptSettings> settings,
        ILogger<BCryptPasswordHasher> logger
    )
    {
        _settings = settings.Value;
        _logger = logger;
    }

    /// <summary>
    /// Hashes a password using BCrypt with salt
    /// </summary>
    /// <param name="password">Plain text password to hash</param>
    /// <returns>BCrypt hashed password with salt</returns>
    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            _logger.LogWarning("Attempted to hash null or empty password");
            throw new ArgumentException("Password cannot be null or empty", nameof(password));
        }

        try
        {
            // Generate salt and hash password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, _settings.WorkFactor);

            _logger.LogDebug(
                "Password hashed successfully with work factor {WorkFactor}",
                _settings.WorkFactor
            );

            return hashedPassword;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to hash password");
            throw new InvalidOperationException("Password hashing failed", ex);
        }
    }

    /// <summary>
    /// Verifies a password against its hash
    /// </summary>
    /// <param name="password">Plain text password to verify</param>
    /// <param name="passwordHash">BCrypt hash to verify against</param>
    /// <returns>True if password matches hash, false otherwise</returns>
    public bool VerifyPassword(string password, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            _logger.LogWarning("Attempted to verify null or empty password");
            return false;
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            _logger.LogWarning("Attempted to verify against null or empty hash");
            return false;
        }

        try
        {
            // Verify password against hash
            var isValid = BCrypt.Net.BCrypt.Verify(password, passwordHash);

            if (!isValid)
            {
                _logger.LogWarning("Password verification failed - invalid credentials");
            }
            else
            {
                _logger.LogDebug("Password verified successfully");
            }

            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Password verification failed due to exception");
            return false;
        }
    }
}
