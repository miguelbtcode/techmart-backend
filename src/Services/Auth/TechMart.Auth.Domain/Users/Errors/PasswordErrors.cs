using TechMart.Auth.Domain.Primitives;

namespace TechMart.Auth.Domain.Users.Errors;

/// <summary>
/// Static error definitions for Password value object
/// </summary>
public static class PasswordErrors
{
    public static Error Empty() => Error.Validation("PASSWORD.EMPTY", "Password cannot be empty");

    public static Error TooShort(int minLength) =>
        Error.Validation(
            "PASSWORD.TOO_SHORT",
            $"Password must be at least {minLength} characters long"
        );

    public static Error TooLong(int maxLength) =>
        Error.Validation("PASSWORD.TOO_LONG", $"Password cannot exceed {maxLength} characters");

    public static Error MissingLowerCase() =>
        Error.Validation(
            "PASSWORD.MISSING_LOWERCASE",
            "Password must contain at least one lowercase letter"
        );

    public static Error MissingUpperCase() =>
        Error.Validation(
            "PASSWORD.MISSING_UPPERCASE",
            "Password must contain at least one uppercase letter"
        );

    public static Error MissingDigit() =>
        Error.Validation("PASSWORD.MISSING_DIGIT", "Password must contain at least one digit");

    public static Error MissingSpecialCharacter() =>
        Error.Validation(
            "PASSWORD.MISSING_SPECIAL",
            "Password must contain at least one special character (@$!%*?&)"
        );

    public static Error WeakPassword(IEnumerable<string> reasons) =>
        Error.Validation("PASSWORD.WEAK", $"Password is too weak: {string.Join(", ", reasons)}");

    public static Error CommonPassword() =>
        Error.Validation("PASSWORD.COMMON", "Password is too common and easily guessable");

    public static Error ContainsPersonalInfo() =>
        Error.Validation("PASSWORD.PERSONAL_INFO", "Password cannot contain personal information");
}
