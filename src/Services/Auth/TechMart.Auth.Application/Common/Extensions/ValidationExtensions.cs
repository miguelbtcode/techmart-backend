using FluentValidation;
using TechMart.Auth.Domain.Users.ValueObjects;

namespace TechMart.Auth.Application.Common.Extensions;

/// <summary>
/// Extension methods for FluentValidation
/// </summary>
public static class ValidationExtensions
{
    /// <summary>
    /// Validates that a GUID is not empty
    /// </summary>
    public static IRuleBuilderOptions<T, Guid> NotEmptyGuid<T>(
        this IRuleBuilder<T, Guid> ruleBuilder
    )
    {
        return ruleBuilder
            .NotEmpty()
            .WithMessage("'{PropertyName}' cannot be empty")
            .WithErrorCode("NotEmpty");
    }

    /// <summary>
    /// Validates that a nullable GUID is not empty if it has a value
    /// </summary>
    public static IRuleBuilderOptions<T, Guid?> NotEmptyGuid<T>(
        this IRuleBuilder<T, Guid?> ruleBuilder
    )
    {
        return ruleBuilder
            .Must(guid => !guid.HasValue || guid.Value != Guid.Empty)
            .WithMessage("'{PropertyName}' cannot be empty if provided")
            .WithErrorCode("NotEmpty");
    }

    /// <summary>
    /// Validates email format using domain Email value object
    /// </summary>
    public static IRuleBuilderOptions<T, string> ValidEmail<T>(
        this IRuleBuilder<T, string> ruleBuilder
    )
    {
        return ruleBuilder
            .NotEmpty()
            .WithMessage("Email is required")
            .WithErrorCode("Required")
            .Must(email => Email.IsValidFormat(email))
            .WithMessage("Invalid email format")
            .WithErrorCode("InvalidFormat");
    }

    /// <summary>
    /// Validates password using domain Password value object
    /// </summary>
    public static IRuleBuilderOptions<T, string> ValidPassword<T>(
        this IRuleBuilder<T, string> ruleBuilder
    )
    {
        return ruleBuilder
            .NotEmpty()
            .WithMessage("Password is required")
            .WithErrorCode("Required")
            .Must(password => Password.IsValid(password))
            .WithMessage("Password does not meet security requirements")
            .WithErrorCode("InvalidFormat");
    }

    /// <summary>
    /// Validates that a string is not empty or whitespace
    /// </summary>
    public static IRuleBuilderOptions<T, string> NotEmptyOrWhitespace<T>(
        this IRuleBuilder<T, string> ruleBuilder
    )
    {
        return ruleBuilder
            .NotEmpty()
            .WithMessage("'{PropertyName}' is required")
            .WithErrorCode("Required")
            .Must(value => !string.IsNullOrWhiteSpace(value))
            .WithMessage("'{PropertyName}' cannot be empty or whitespace")
            .WithErrorCode("NotEmpty");
    }

    /// <summary>
    /// Validates string length with custom error messages
    /// </summary>
    public static IRuleBuilderOptions<T, string> LengthBetween<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        int min,
        int max
    )
    {
        return ruleBuilder
            .Length(min, max)
            .WithMessage($"'{{PropertyName}}' must be between {min} and {max} characters")
            .WithErrorCode("InvalidLength");
    }

    /// <summary>
    /// Validates that two properties match (useful for password confirmation)
    /// </summary>
    public static IRuleBuilderOptions<T, string> MustMatch<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        Func<T, string> comparisonProperty,
        string comparisonPropertyName
    )
    {
        return ruleBuilder
            .Must((instance, value) => value == comparisonProperty(instance))
            .WithMessage($"'{{PropertyName}}' must match {comparisonPropertyName}")
            .WithErrorCode("MustMatch");
    }

    /// <summary>
    /// Validates that a collection is not empty
    /// </summary>
    public static IRuleBuilderOptions<T, IEnumerable<TElement>> NotEmptyCollection<T, TElement>(
        this IRuleBuilder<T, IEnumerable<TElement>> ruleBuilder
    )
    {
        return ruleBuilder
            .NotEmpty()
            .WithMessage("'{PropertyName}' cannot be empty")
            .WithErrorCode("NotEmpty")
            .Must(collection => collection?.Any() == true)
            .WithMessage("'{PropertyName}' must contain at least one item")
            .WithErrorCode("MinLength");
    }

    /// <summary>
    /// Validates enum values
    /// </summary>
    public static IRuleBuilderOptions<T, TEnum> ValidEnum<T, TEnum>(
        this IRuleBuilder<T, TEnum> ruleBuilder
    )
        where TEnum : struct, Enum
    {
        return ruleBuilder
            .Must(value => Enum.IsDefined(typeof(TEnum), value))
            .WithMessage("'{PropertyName}' has an invalid value")
            .WithErrorCode("InvalidEnum");
    }

    /// <summary>
    /// Validates that a date is in the past
    /// </summary>
    public static IRuleBuilderOptions<T, DateTime> InThePast<T>(
        this IRuleBuilder<T, DateTime> ruleBuilder
    )
    {
        return ruleBuilder
            .Must(date => date < DateTime.UtcNow)
            .WithMessage("'{PropertyName}' must be in the past")
            .WithErrorCode("InvalidDate");
    }

    /// <summary>
    /// Validates that a date is in the future
    /// </summary>
    public static IRuleBuilderOptions<T, DateTime> InTheFuture<T>(
        this IRuleBuilder<T, DateTime> ruleBuilder
    )
    {
        return ruleBuilder
            .Must(date => date > DateTime.UtcNow)
            .WithMessage("'{PropertyName}' must be in the future")
            .WithErrorCode("InvalidDate");
    }
}
