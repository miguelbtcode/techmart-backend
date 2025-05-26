using TechMart.Auth.Domain.Primitives;

namespace TechMart.Auth.Domain.Users.Errors;

/// <summary>
/// Static error definitions for Email value object
/// </summary>
public static class EmailErrors
{
    public static Error Empty() => Error.Validation("EMAIL.EMPTY", "Email address cannot be empty");

    public static Error TooLong(int maxLength) =>
        Error.Validation("EMAIL.TOO_LONG", $"Email address cannot exceed {maxLength} characters");

    public static Error InvalidFormat(string email) =>
        Error.Validation("EMAIL.INVALID_FORMAT", $"'{email}' is not a valid email format");

    public static Error InvalidDomain(string domain) =>
        Error.Validation("EMAIL.INVALID_DOMAIN", $"Email domain '{domain}' is not valid");

    public static Error Blocked(string email) =>
        Error.Failure("EMAIL.BLOCKED", $"Email address '{email}' is blocked");
}
