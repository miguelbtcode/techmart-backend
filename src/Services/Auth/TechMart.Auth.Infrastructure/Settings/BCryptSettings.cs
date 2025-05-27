namespace TechMart.Auth.Infrastructure.Settings;

/// <summary>
/// Configuration settings for BCrypt password hashing
/// </summary>
public sealed class BCryptSettings
{
    public const string SectionName = "BCryptSettings";

    /// <summary>
    /// Work factor (cost) for BCrypt hashing
    /// Higher values are more secure but slower
    /// Recommended: 12-15 for production
    /// </summary>
    public int WorkFactor { get; set; } = 12;

    /// <summary>
    /// Validates the BCrypt settings
    /// </summary>
    public void Validate()
    {
        if (WorkFactor < 4 || WorkFactor > 20)
        {
            throw new ArgumentOutOfRangeException(
                nameof(WorkFactor),
                WorkFactor,
                "Work factor must be between 4 and 20"
            );
        }
    }
}
