namespace TechMart.Auth.Application.Contracts.Authentication;

/// <summary>
/// Service for hashing and verifying passwords
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hashes a plain text password
    /// </summary>
    string HashPassword(string password);

    /// <summary>
    /// Verifies a password against its hash
    /// </summary>
    bool VerifyPassword(string password, string passwordHash);

    /// <summary>
    /// Checks if a password hash needs to be rehashed (for security upgrades)
    /// </summary>
    bool NeedsRehash(string passwordHash);

    /// <summary>
    /// Gets the strength/cost factor of a password hash
    /// </summary>
    int GetHashStrength(string passwordHash);
}
