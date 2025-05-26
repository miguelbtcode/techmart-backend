using System.Text.RegularExpressions;
using TechMart.Auth.Domain.Primitives;
using TechMart.Auth.Domain.Users.enums;
using TechMart.Auth.Domain.Users.Errors;

namespace TechMart.Auth.Domain.Users.ValueObjects;

/// <summary>
/// Value object representing a valid password
/// </summary>
public sealed class Password : IEquatable<Password>
{
    private static readonly Regex HasLowerCase = new(@"[a-z]", RegexOptions.Compiled);
    private static readonly Regex HasUpperCase = new(@"[A-Z]", RegexOptions.Compiled);
    private static readonly Regex HasDigit = new(@"\d", RegexOptions.Compiled);
    private static readonly Regex HasSpecialChar = new(@"[@$!%*?&]", RegexOptions.Compiled);

    private Password(string value)
    {
        Value = value;
    }

    /// <summary>
    /// The password value (should be hashed before storage)
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Creates a new Password instance with validation
    /// </summary>
    public static Result<Password> Create(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return PasswordErrors.Empty();

        if (password.Length < 8)
            return PasswordErrors.TooShort(8);

        if (password.Length > 100)
            return PasswordErrors.TooLong(100);

        if (!HasLowerCase.IsMatch(password))
            return PasswordErrors.MissingLowerCase();

        if (!HasUpperCase.IsMatch(password))
            return PasswordErrors.MissingUpperCase();

        if (!HasDigit.IsMatch(password))
            return PasswordErrors.MissingDigit();

        if (!HasSpecialChar.IsMatch(password))
            return PasswordErrors.MissingSpecialCharacter();

        return new Password(password);
    }

    /// <summary>
    /// Gets the strength of the current password
    /// </summary>
    public PasswordStrength GetStrength()
    {
        return GetStrength(Value);
    }

    /// <summary>
    /// Gets the strength of a password
    /// </summary>
    public static PasswordStrength GetStrength(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return PasswordStrength.VeryWeak;

        var score = 0;

        // Length scoring
        if (password.Length >= 8)
            score++;
        if (password.Length >= 12)
            score++;
        if (password.Length >= 16)
            score++;

        // Character variety scoring
        if (HasLowerCase.IsMatch(password))
            score++;
        if (HasUpperCase.IsMatch(password))
            score++;
        if (HasDigit.IsMatch(password))
            score++;
        if (HasSpecialChar.IsMatch(password))
            score++;

        // Additional complexity
        if (password.Length >= 20)
            score++;
        if (Regex.IsMatch(password, @"[^a-zA-Z0-9@$!%*?&]"))
            score++;

        return score switch
        {
            <= 3 => PasswordStrength.VeryWeak,
            4 => PasswordStrength.Weak,
            5 => PasswordStrength.Medium,
            6 => PasswordStrength.Strong,
            >= 7 => PasswordStrength.VeryStrong,
        };
    }

    /// <summary>
    /// Validates password requirements without creating instance
    /// </summary>
    public static bool IsValid(string password)
    {
        var result = Create(password);
        return result.IsSuccess;
    }

    #region Equality Implementation

    public bool Equals(Password? other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return Value == other.Value;
    }

    public override bool Equals(object? obj) => Equals(obj as Password);

    public override int GetHashCode() => Value.GetHashCode();

    public static bool operator ==(Password? left, Password? right) => Equals(left, right);

    public static bool operator !=(Password? left, Password? right) => !Equals(left, right);

    #endregion

    /// <summary>
    /// Never expose the actual password value
    /// </summary>
    public override string ToString() => "***";
}
