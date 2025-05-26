using System.Text.RegularExpressions;
using TechMart.Auth.Domain.Primitives;
using TechMart.Auth.Domain.Users.Errors;

namespace TechMart.Auth.Domain.Users.ValueObjects;

/// <summary>
/// Value object representing a valid email address
/// </summary>
public sealed class Email : IEquatable<Email>
{
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );

    private Email(string value)
    {
        Value = value;
    }

    /// <summary>
    /// The email address value
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Creates a new Email instance with validation
    /// </summary>
    public static Result<Email> Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return EmailErrors.Empty();

        email = email.Trim().ToLowerInvariant();

        if (email.Length > 255)
            return EmailErrors.TooLong(255);

        if (!IsValidFormat(email))
            return EmailErrors.InvalidFormat(email);

        return new Email(email);
    }

    /// <summary>
    /// Validates email format without creating instance
    /// </summary>
    public static bool IsValidFormat(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || email.Length > 255)
            return false;

        return EmailRegex.IsMatch(email);
    }

    /// <summary>
    /// Gets the domain part of the email
    /// </summary>
    public string GetDomain()
    {
        var atIndex = Value.IndexOf('@');
        return atIndex > 0 ? Value.Substring(atIndex + 1) : string.Empty;
    }

    /// <summary>
    /// Gets the local part of the email (before @)
    /// </summary>
    public string GetLocalPart()
    {
        var atIndex = Value.IndexOf('@');
        return atIndex > 0 ? Value.Substring(0, atIndex) : Value;
    }

    /// <summary>
    /// Checks if email is from a specific domain
    /// </summary>
    public bool IsFromDomain(string domain)
    {
        return GetDomain().Equals(domain, StringComparison.OrdinalIgnoreCase);
    }

    #region Equality Implementation

    public bool Equals(Email? other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return Value == other.Value;
    }

    public override bool Equals(object? obj) => Equals(obj as Email);

    public override int GetHashCode() => Value.GetHashCode();

    public static bool operator ==(Email? left, Email? right) => Equals(left, right);

    public static bool operator !=(Email? left, Email? right) => !Equals(left, right);

    #endregion

    public override string ToString() => Value;

    public static implicit operator string(Email email) => email?.Value ?? string.Empty;
}
