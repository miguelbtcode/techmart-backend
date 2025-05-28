using System.Linq.Expressions;
using FluentValidation;
using TechMart.Auth.Application.Common.Extensions;

namespace TechMart.Auth.Application.Common.Validators;

/// <summary>
/// Base validator with common validation rules and consistent error messages
/// </summary>
/// <typeparam name="T">Type being validated</typeparam>
public abstract class BaseValidator<T> : AbstractValidator<T>
{
    protected BaseValidator() =>
        // Set default cascade mode to stop on first failure per property
        ClassLevelCascadeMode = FluentValidation.CascadeMode.Stop;

    #region User ID Validation

    /// <summary>
    /// Validates a required user ID property
    /// </summary>
    protected void ValidateUserId(
        Expression<Func<T, Guid>> propertySelector,
        string propertyName = "User ID"
    )
    {
        RuleFor(propertySelector)
            .NotEmptyGuid()
            .WithMessage($"{propertyName} is required")
            .WithErrorCode($"{propertyName.Replace(" ", "")}.Required");
    }

    /// <summary>
    /// Validates an optional user ID property
    /// </summary>
    protected void ValidateOptionalUserId(
        Expression<Func<T, Guid?>> propertySelector,
        string propertyName = "User ID"
    )
    {
        When(
            instance => propertySelector.Compile()(instance).HasValue,
            () =>
            {
                RuleFor(propertySelector)
                    .Must(id => id.HasValue && id.Value != Guid.Empty)
                    .WithMessage($"{propertyName} cannot be empty if provided")
                    .WithErrorCode($"{propertyName.Replace(" ", "")}.Invalid");
            }
        );
    }

    #endregion

    #region Email and Password Validation

    /// <summary>
    /// Validates an email property
    /// </summary>
    protected void ValidateEmail(
        Expression<Func<T, string>> propertySelector,
        string propertyName = "Email"
    )
    {
        RuleFor(propertySelector).ValidEmail().WithName(propertyName);
    }

    /// <summary>
    /// Validates a password property
    /// </summary>
    protected void ValidatePassword(
        Expression<Func<T, string>> propertySelector,
        string propertyName = "Password"
    )
    {
        RuleFor(propertySelector).ValidPassword().WithName(propertyName);
    }

    /// <summary>
    /// Validates password confirmation
    /// </summary>
    protected void ValidatePasswordConfirmation(
        Expression<Func<T, string>> passwordSelector,
        Expression<Func<T, string>> confirmPasswordSelector,
        string confirmPasswordName = "Password confirmation"
    )
    {
        RuleFor(confirmPasswordSelector)
            .NotEmpty()
            .WithMessage($"{confirmPasswordName} is required")
            .WithErrorCode($"{confirmPasswordName.Replace(" ", "")}.Required")
            .MustMatch(passwordSelector.Compile(), "Password")
            .WithMessage($"{confirmPasswordName} must match password")
            .WithErrorCode($"{confirmPasswordName.Replace(" ", "")}.MustMatch");
    }

    #endregion

    #region String Validation

    /// <summary>
    /// Validates a required string property
    /// </summary>
    protected void ValidateRequiredString(
        Expression<Func<T, string>> propertySelector,
        string propertyName,
        int maxLength = 255
    )
    {
        RuleFor(propertySelector)
            .NotEmptyOrWhitespace()
            .WithMessage($"{propertyName} is required")
            .WithErrorCode($"{propertyName.Replace(" ", "")}.Required")
            .MaximumLength(maxLength)
            .WithMessage($"{propertyName} cannot exceed {maxLength} characters")
            .WithErrorCode($"{propertyName.Replace(" ", "")}.TooLong");
    }

    /// <summary>
    /// Validates a name property (first name, last name, etc.)
    /// </summary>
    protected void ValidateName(Expression<Func<T, string>> propertySelector, string propertyName)
    {
        RuleFor(propertySelector)
            .NotEmptyOrWhitespace()
            .WithMessage($"{propertyName} is required")
            .WithErrorCode($"{propertyName.Replace(" ", "")}.Required")
            .LengthBetween(1, ValidationRules.Lengths.NameMaxLength)
            .WithMessage(
                $"{propertyName} must be between 1 and {ValidationRules.Lengths.NameMaxLength} characters"
            )
            .WithErrorCode($"{propertyName.Replace(" ", "")}.InvalidLength")
            .Matches(ValidationRules.Patterns.Name)
            .WithMessage(
                $"{propertyName} can only contain letters, spaces, hyphens and apostrophes"
            )
            .WithErrorCode($"{propertyName.Replace(" ", "")}.InvalidCharacters");
    }

    #endregion

    #region Pagination and Collections

    /// <summary>
    /// Validates pagination parameters
    /// </summary>
    protected void ValidatePagination(
        Expression<Func<T, int>> pageIndexSelector,
        Expression<Func<T, int>> pageSizeSelector
    )
    {
        RuleFor(pageIndexSelector)
            .GreaterThan(0)
            .WithMessage("Page index must be greater than 0")
            .WithErrorCode("PageIndex.Invalid");

        RuleFor(pageSizeSelector)
            .InclusiveBetween(
                ValidationRules.Pagination.MinPageSize,
                ValidationRules.Pagination.MaxPageSize
            )
            .WithMessage("Page size must be between 1 and 100")
            .WithErrorCode("PageSize.Invalid");
    }

    /// <summary>
    /// Validates that a collection is not empty
    /// </summary>
    protected void ValidateNonEmptyCollection<TItem>(
        Expression<Func<T, IEnumerable<TItem>>> propertySelector,
        string propertyName
    )
    {
        RuleFor(propertySelector)
            .NotEmptyCollection()
            .WithMessage($"{propertyName} cannot be empty")
            .WithErrorCode($"{propertyName.Replace(" ", "")}.Empty");
    }

    #endregion

    #region Enum and Token Validation

    /// <summary>
    /// Validates an enum property
    /// </summary>
    protected void ValidateEnum<TEnum>(
        Expression<Func<T, TEnum>> propertySelector,
        string propertyName
    )
        where TEnum : struct, Enum
    {
        RuleFor(propertySelector)
            .ValidEnum()
            .WithMessage($"{propertyName} has an invalid value")
            .WithErrorCode($"{propertyName.Replace(" ", "")}.Invalid");
    }

    /// <summary>
    /// Validates a token property
    /// </summary>
    protected void ValidateToken(
        Expression<Func<T, string>> propertySelector,
        string tokenName = "Token"
    )
    {
        RuleFor(propertySelector)
            .NotEmptyOrWhitespace()
            .WithMessage($"{tokenName} is required")
            .WithErrorCode($"{tokenName}.Required")
            .MinimumLength(10)
            .WithMessage($"{tokenName} is too short")
            .WithErrorCode($"{tokenName}.TooShort");
    }

    #endregion

    #region Date Validation

    /// <summary>
    /// Validates that a date is in the past
    /// </summary>
    protected void ValidatePastDate(
        Expression<Func<T, DateTime>> propertySelector,
        string propertyName
    )
    {
        RuleFor(propertySelector)
            .InThePast()
            .WithMessage($"{propertyName} must be in the past")
            .WithErrorCode($"{propertyName.Replace(" ", "")}.InvalidDate");
    }

    /// <summary>
    /// Validates that a date is in the future
    /// </summary>
    protected void ValidateFutureDate(
        Expression<Func<T, DateTime>> propertySelector,
        string propertyName
    )
    {
        RuleFor(propertySelector)
            .InTheFuture()
            .WithMessage($"{propertyName} must be in the future")
            .WithErrorCode($"{propertyName.Replace(" ", "")}.InvalidDate");
    }

    #endregion
}
