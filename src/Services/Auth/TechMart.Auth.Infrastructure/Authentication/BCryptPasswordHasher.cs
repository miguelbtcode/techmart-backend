using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TechMart.Auth.Application.Contracts.Authentication;
using TechMart.Auth.Infrastructure.Settings;

namespace TechMart.Auth.Infrastructure.Authentication;

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

    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            _logger.LogWarning("Attempted to hash null or empty password");
            throw new ArgumentException("Password cannot be null or empty", nameof(password));
        }

        try
        {
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

    public bool VerifyPassword(string password, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(passwordHash))
        {
            _logger.LogWarning("Attempted to verify with null or empty values");
            return false;
        }

        try
        {
            var isValid = BCrypt.Net.BCrypt.Verify(password, passwordHash);
            if (!isValid)
            {
                _logger.LogWarning("Password verification failed - invalid credentials");
            }
            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Password verification failed due to exception");
            return false;
        }
    }

    // Implementar métodos faltantes
    public bool NeedsRehash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            return true;

        try
        {
            // BCrypt stores the work factor in the hash itself
            // If the current work factor is higher than the hash's work factor,
            // then we need to rehash
            var currentWorkFactor = GetHashStrength(passwordHash);
            return currentWorkFactor < _settings.WorkFactor;
        }
        catch
        {
            // If we can't determine the work factor, assume rehash is needed
            return true;
        }
    }

    public int GetHashStrength(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            return 0;

        try
        {
            // BCrypt hash format: $2a$10$... where 10 is the work factor
            var parts = passwordHash.Split('$');
            if (parts.Length >= 3 && int.TryParse(parts[2], out var workFactor))
            {
                return workFactor;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to extract work factor from password hash");
        }

        return 0;
    }
}
