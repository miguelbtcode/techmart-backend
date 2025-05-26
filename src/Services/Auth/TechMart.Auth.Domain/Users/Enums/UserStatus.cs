namespace TechMart.Auth.Domain.Users.Enums;

/// <summary>
/// Represents the status of a user account
/// </summary>
public enum UserStatus
{
    /// <summary>
    /// User account is active and can login
    /// </summary>
    Active = 1,

    /// <summary>
    /// User account is temporarily inactive
    /// </summary>
    Inactive = 2,

    /// <summary>
    /// User account is suspended due to violations
    /// </summary>
    Suspended = 3,

    /// <summary>
    /// User account is pending email confirmation
    /// </summary>
    PendingEmailConfirmation = 4,

    /// <summary>
    /// User account is soft deleted
    /// </summary>
    Deleted = 5,
}
