using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace TechMart.SharedKernel.Extensions.String;

/// <summary>
/// Extension methods for string manipulation.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Checks if the string is null or empty.
    /// </summary>
    /// <param name="value">The string value.</param>
    /// <returns>True if null or empty; otherwise, false.</returns>
    public static bool IsNullOrEmpty(this string? value) => string.IsNullOrEmpty(value);

    /// <summary>
    /// Checks if the string is null, empty, or whitespace.
    /// </summary>
    /// <param name="value">The string value.</param>
    /// <returns>True if null, empty, or whitespace; otherwise, false.</returns>
    public static bool IsNullOrWhiteSpace(this string? value) => string.IsNullOrWhiteSpace(value);

    /// <summary>
    /// Checks if the string has a value (not null, empty, or whitespace).
    /// </summary>
    /// <param name="value">The string value.</param>
    /// <returns>True if has value; otherwise, false.</returns>
    public static bool HasValue(this string? value) => !string.IsNullOrWhiteSpace(value);

    /// <summary>
    /// Converts the string to camelCase.
    /// </summary>
    /// <param name="value">The string value.</param>
    /// <returns>The camelCase string.</returns>
    public static string ToCamelCase(this string value)
    {
        if (value.IsNullOrEmpty()) return value;
        
        return char.ToLowerInvariant(value[0]) + value[1..];
    }

    /// <summary>
    /// Converts the string to PascalCase.
    /// </summary>
    /// <param name="value">The string value.</param>
    /// <returns>The PascalCase string.</returns>
    public static string ToPascalCase(this string value)
    {
        if (value.IsNullOrEmpty()) return value;
        
        return char.ToUpperInvariant(value[0]) + value[1..];
    }

    /// <summary>
    /// Converts the string to kebab-case.
    /// </summary>
    /// <param name="value">The string value.</param>
    /// <returns>The kebab-case string.</returns>
    public static string ToKebabCase(this string value)
    {
        if (value.IsNullOrEmpty()) return value;

        return Regex.Replace(value, "([a-z])([A-Z])", "$1-$2").ToLowerInvariant();
    }

    /// <summary>
    /// Converts the string to snake_case.
    /// </summary>
    /// <param name="value">The string value.</param>
    /// <returns>The snake_case string.</returns>
    public static string ToSnakeCase(this string value)
    {
        if (value.IsNullOrEmpty()) return value;

        return Regex.Replace(value, "([a-z])([A-Z])", "$1_$2").ToLowerInvariant();
    }

    /// <summary>
    /// Truncates the string to the specified length.
    /// </summary>
    /// <param name="value">The string value.</param>
    /// <param name="maxLength">The maximum length.</param>
    /// <param name="suffix">The suffix to add if truncated.</param>
    /// <returns>The truncated string.</returns>
    public static string Truncate(this string value, int maxLength, string suffix = "...")
    {
        if (value.IsNullOrEmpty() || value.Length <= maxLength) return value;
        
        return value[..(maxLength - suffix.Length)] + suffix;
    }

    /// <summary>
    /// Masks the string with the specified character.
    /// </summary>
    /// <param name="value">The string value.</param>
    /// <param name="maskChar">The mask character.</param>
    /// <param name="visibleStart">Number of characters to show at start.</param>
    /// <param name="visibleEnd">Number of characters to show at end.</param>
    /// <returns>The masked string.</returns>
    public static string Mask(this string value, char maskChar = '*', int visibleStart = 2, int visibleEnd = 2)
    {
        if (value.IsNullOrEmpty() || value.Length <= visibleStart + visibleEnd) return value;

        var start = value[..visibleStart];
        var end = value[^visibleEnd..];
        var middle = new string(maskChar, value.Length - visibleStart - visibleEnd);
        
        return start + middle + end;
    }

    /// <summary>
    /// Removes diacritics (accents) from the string.
    /// </summary>
    /// <param name="value">The string value.</param>
    /// <returns>The string without diacritics.</returns>
    public static string RemoveDiacritics(this string value)
    {
        if (value.IsNullOrEmpty()) return value;

        var normalizedString = value.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }

    /// <summary>
    /// Converts the string to a URL-friendly slug.
    /// </summary>
    /// <param name="value">The string value.</param>
    /// <returns>The URL slug.</returns>
    public static string ToSlug(this string value)
    {
        if (value.IsNullOrEmpty()) return value;

        // Remove diacritics and convert to lowercase
        var slug = value.RemoveDiacritics().ToLowerInvariant();
        
        // Replace spaces and special characters with hyphens
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = Regex.Replace(slug, @"-+", "-");
        
        return slug.Trim('-');
    }

    /// <summary>
    /// Checks if the string is a valid email address.
    /// </summary>
    /// <param name="value">The string value.</param>
    /// <returns>True if valid email; otherwise, false.</returns>
    public static bool IsValidEmail(this string value)
    {
        if (value.IsNullOrWhiteSpace()) return false;

        try
        {
            return Regex.IsMatch(value,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }

    /// <summary>
    /// Extracts numbers from the string.
    /// </summary>
    /// <param name="value">The string value.</param>
    /// <returns>The extracted numbers as string.</returns>
    public static string ExtractNumbers(this string value)
    {
        if (value.IsNullOrEmpty()) return string.Empty;
        
        return Regex.Replace(value, @"[^\d]", "");
    }

    /// <summary>
    /// Extracts letters from the string.
    /// </summary>
    /// <param name="value">The string value.</param>
    /// <returns>The extracted letters as string.</returns>
    public static string ExtractLetters(this string value)
    {
        if (value.IsNullOrEmpty()) return string.Empty;
        
        return Regex.Replace(value, @"[^a-zA-Z]", "");
    }

    /// <summary>
    /// Repeats the string the specified number of times.
    /// </summary>
    /// <param name="value">The string value.</param>
    /// <param name="count">The number of times to repeat.</param>
    /// <returns>The repeated string.</returns>
    public static string Repeat(this string value, int count)
    {
        if (value.IsNullOrEmpty() || count <= 0) return string.Empty;
        
        return string.Concat(Enumerable.Repeat(value, count));
    }
}