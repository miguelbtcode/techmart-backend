using FluentValidation;
using TechMart.Auth.Application.Common.Extensions;

namespace TechMart.Auth.Application.Common.Validators;

/// <summary>
/// Base validator with common validation rules
/// </summary>
/// <typeparam name="T">Type being validated</typeparam>
public abstract class BaseValidator<T> : AbstractValidator<T>
{
    protected BaseValidator()
    {
        // Set default cascade mode to stop on first failure per property
        CascadeMode = CascadeMode.Stop;
    }

    /// <summary>
    /// Validates a user ID property
    /// </summary>
    protected void ValidateUserId<TProperty>(Func<T, TProperty> propertySelector)
        where TProperty : Guid
    {
        RuleFor(propertySelector)
            .NotEmptyGuid()
            .WithMessage("User ID is required")
            .WithErrorCode("UserId.Required");
    }

    /// <summary>
    /// Validates an optional user ID property
    /// </summary>
    protected void ValidateOptionalUserId<TProperty>(Func<T, TProperty?> propertySelector)
        where TProperty : struct
    {
        When(
            instance => propertySelector(instance).HasValue,
            () =>
            {
                RuleFor(propertySelector)
                    .Must(id => id.HasValue && !id.Value.Equals(Guid.Empty))
                    .WithMessage("User ID cannot be empty if provided")
                    .WithErrorCode("UserId.Invalid");
            }
        );
    }

    /// <summary>
    /// Validates an email property
    /// </summary>
    protected void ValidateEmail(Func<T, string> propertySelector)
    {
        RuleFor(propertySelector).ValidEmail();
    }

    /// <summary>
    /// Validates a password property
    /// </summary>
    protected void ValidatePassword(Func<T, string> propertySelector)
    {
        RuleFor(propertySelector).ValidPassword();
    }

    /// <summary>
    /// Validates password confirmation
    /// </summary>
    protected void ValidatePasswordConfirmation(
        Func<T, string> passwordSelector,
        Func<T, string> confirmPasswordSelector
    )
    {
        RuleFor(confirmPasswordSelector)
            .NotEmpty()
            .WithMessage("Password confirmation is required")
            .WithErrorCode("ConfirmPassword.Required")
            .MustMatch(passwordSelector, "Password")
            .WithMessage("Password confirmation must match password")
            .WithErrorCode("ConfirmPassword.MustMatch");
    }

    /// <summary>
    /// Validates a required string property
    /// </summary>
    protected void ValidateRequiredString(
        Func<T, string> propertySelector,
        string propertyName,
        int maxLength = 255
    )
    {
        RuleFor(propertySelector)
            .NotEmptyOrWhitespace()
            .WithMessage($"{propertyName} is required")
            .WithErrorCode($"{propertyName}.Required")
            .MaximumLength(maxLength)
            .WithMessage($"{propertyName} cannot exceed {maxLength} characters")
            .WithErrorCode($"{propertyName}.TooLong");
    }

    /// <summary>
    /// Validates a name property (first name, last name, etc.)
    /// </summary>
    protected void ValidateName(Func<T, string> propertySelector, string propertyName)
    {
        RuleFor(propertySelector)
            .NotEmptyOrWhitespace()
            .WithMessage($"{propertyName} is required")
            .WithErrorCode($"{propertyName}.Required")
            .LengthBetween(1, 100)
            .WithMessage($"{propertyName} must be between 1 and 100 characters")
            .WithErrorCode($"{propertyName}.InvalidLength")
            .Matches(@"^[a-zA-ZÀ-ÿ\s'-]+$")
            .WithMessage(
                $"{propertyName} can only contain letters, spaces, hyphens and apostrophes"
            )
            .WithErrorCode($"{propertyName}.InvalidCharacters");
    }

    /// <summary>
    /// Validates pagination parameters
    /// </summary>
    protected void ValidatePagination(Func<T, int> pageIndexSelector, Func<T, int> pageSizeSelector)
    {
        RuleFor(pageIndexSelector)
            .GreaterThan(0)
            .WithMessage("Page index must be greater than 0")
            .WithErrorCode("PageIndex.Invalid");

        RuleFor(pageSizeSelector)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100")
            .WithErrorCode("PageSize.Invalid");
    }

    /// <summary>
    /// Validates an enum property
    /// </summary>
    protected void ValidateEnum<TEnum>(Func<T, TEnum> propertySelector, string propertyName)
        where TEnum : struct, Enum
    {
        RuleFor(propertySelector)
            .ValidEnum()
            .WithMessage($"{propertyName} has an invalid value")
            .WithErrorCode($"{propertyName}.Invalid");
    }

    /// <summary>
    /// Validates a token property
    /// </summary>
    protected void ValidateToken(Func<T, string> propertySelector, string tokenName = "Token")
    {
        RuleFor(propertySelector)
            .NotEmptyOrWhitespace()
            .WithMessage($"{tokenName} is required")
            .WithErrorCode($"{tokenName}.Required")
            .MinimumLength(10)
            .WithMessage($"{tokenName} is too short")
            .WithErrorCode($"{tokenName}.TooShort");
    }
}
