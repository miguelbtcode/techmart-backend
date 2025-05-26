namespace TechMart.Auth.Domain.Users.enums;

/// <summary>
/// Represents password strength levels
/// </summary>
public enum PasswordStrength
{
    /// <summary>
    /// Very weak password - does not meet basic requirements
    /// </summary>
    VeryWeak = 1,

    /// <summary>
    /// Weak password - meets some basic requirements
    /// </summary>
    Weak = 2,

    /// <summary>
    /// Medium strength password - meets most requirements
    /// </summary>
    Medium = 3,

    /// <summary>
    /// Strong password - meets all requirements with good complexity
    /// </summary>
    Strong = 4,

    /// <summary>
    /// Very strong password - exceeds all requirements
    /// </summary>
    VeryStrong = 5,
}
