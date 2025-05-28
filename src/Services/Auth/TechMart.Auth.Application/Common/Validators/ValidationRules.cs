using System.Text.RegularExpressions;

namespace TechMart.Auth.Application.Common.Validators;

/// <summary>
/// Common validation rules and constants
/// </summary>
public static class ValidationRules
{
    /// <summary>
    /// Regular expressions for validation
    /// </summary>
    public static class Patterns
    {
        /// <summary>
        /// Pattern for names (letters, spaces, hyphens, apostrophes, accented characters)
        /// </summary>
        public static readonly Regex Name = new(@"^[a-zA-ZÀ-ÿ\s'-]+$", RegexOptions.Compiled);

        /// <summary>
        /// Pattern for alphanumeric with spaces
        /// </summary>
        public static readonly Regex AlphanumericWithSpaces = new(
            @"^[a-zA-Z0-9\s]+$",
            RegexOptions.Compiled
        );

        /// <summary>
        /// Pattern for strong password validation (alternative to domain Password)
        /// </summary>
        public static readonly Regex StrongPassword = new(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            RegexOptions.Compiled
        );

        /// <summary>
        /// Pattern for URL validation
        /// </summary>
        public static readonly Regex Url = new(
            @"^https?:\/\/.+",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        /// <summary>
        /// Pattern for phone number (basic international format)
        /// </summary>
        public static readonly Regex PhoneNumber = new(
            @"^\+?[1-9]\d{1,14}$",
            RegexOptions.Compiled
        );
    }

    /// <summary>
    /// Length constraints
    /// </summary>
    public static class Lengths
    {
        public const int EmailMaxLength = 255;
        public const int NameMaxLength = 100;
        public const int PasswordMinLength = 8;
        public const int PasswordMaxLength = 100;
        public const int TokenMinLength = 10;
        public const int DescriptionMaxLength = 500;
        public const int CommentMaxLength = 1000;
    }

    /// <summary>
    /// Pagination constraints
    /// </summary>
    public static class Pagination
    {
        public const int MinPageIndex = 1;
        public const int MinPageSize = 1;
        public const int MaxPageSize = 100;
        public const int DefaultPageSize = 10;
    }

    /// <summary>
    /// Error codes for consistent validation messages
    /// </summary>
    public static class ErrorCodes
    {
        public const string Required = "Required";
        public const string InvalidFormat = "InvalidFormat";
        public const string TooShort = "TooShort";
        public const string TooLong = "TooLong";
        public const string InvalidLength = "InvalidLength";
        public const string InvalidCharacters = "InvalidCharacters";
        public const string MustMatch = "MustMatch";
        public const string NotEmpty = "NotEmpty";
        public const string Invalid = "Invalid";
        public const string OutOfRange = "OutOfRange";
    }

    /// <summary>
    /// Common validation methods
    /// </summary>
    public static class Methods
    {
        /// <summary>
        /// Validates if a string is a valid name
        /// </summary>
        public static bool IsValidName(string? name)
        {
            return !string.IsNullOrWhiteSpace(name)
                && name.Length <= Lengths.NameMaxLength
                && Patterns.Name.IsMatch(name);
        }

        /// <summary>
        /// Validates if a string is a valid URL
        /// </summary>
        public static bool IsValidUrl(string? url)
        {
            return !string.IsNullOrWhiteSpace(url)
                && Uri.TryCreate(url, UriKind.Absolute, out _)
                && Patterns.Url.IsMatch(url);
        }

        /// <summary>
        /// Validates if a string is a valid phone number
        /// </summary>
        public static bool IsValidPhoneNumber(string? phoneNumber)
        {
            return !string.IsNullOrWhiteSpace(phoneNumber)
                && Patterns.PhoneNumber.IsMatch(phoneNumber);
        }

        /// <summary>
        /// Sanitizes search terms
        /// </summary>
        public static string? SanitizeSearchTerm(string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return null;

            // Remove special characters that could cause issues
            var sanitized = searchTerm.Trim();

            // Remove or escape SQL injection attempts
            sanitized = sanitized.Replace("'", "''");
            sanitized = sanitized.Replace(";", "");
            sanitized = sanitized.Replace("--", "");
            sanitized = sanitized.Replace("/*", "");
            sanitized = sanitized.Replace("*/", "");

            return sanitized.Length > 0 ? sanitized : null;
        }

        /// <summary>
        /// Validates sort field name
        /// </summary>
        public static bool IsValidSortField(string? sortField, IEnumerable<string> allowedFields)
        {
            if (string.IsNullOrWhiteSpace(sortField))
                return false;

            return allowedFields.Contains(sortField, StringComparer.OrdinalIgnoreCase);
        }
    }
}
