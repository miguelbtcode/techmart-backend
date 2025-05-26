using TechMart.Auth.Domain.Primitives;
using TechMart.Auth.Domain.Users.Enums;

namespace TechMart.Auth.Domain.Users.Errors;

/// <summary>
/// Static error definitions for User entity
/// </summary>
public static class UserErrors
{
    public static Error NotFound(Guid userId) =>
        Error.NotFound("USER.NOT_FOUND", $"User with ID '{userId}' was not found");

    public static Error NotFoundByEmail(string email) =>
        Error.NotFound("USER.NOT_FOUND", $"User with email '{email}' was not found");

    public static Error EmailAlreadyExists(string email) =>
        Error.Conflict("USER.EMAIL_EXISTS", $"A user with email '{email}' already exists");

    public static Error FirstNameRequired() =>
        Error.Validation("USER.FIRST_NAME_REQUIRED", "First name is required");

    public static Error LastNameRequired() =>
        Error.Validation("USER.LAST_NAME_REQUIRED", "Last name is required");

    public static Error FirstNameTooLong(int maxLength) =>
        Error.Validation(
            "USER.FIRST_NAME_TOO_LONG",
            $"First name cannot exceed {maxLength} characters"
        );

    public static Error LastNameTooLong(int maxLength) =>
        Error.Validation(
            "USER.LAST_NAME_TOO_LONG",
            $"Last name cannot exceed {maxLength} characters"
        );

    public static Error PasswordHashRequired() =>
        Error.Validation("USER.PASSWORD_HASH_REQUIRED", "Password hash is required");

    public static Error PasswordHashTooLong(int maxLength) =>
        Error.Validation(
            "USER.PASSWORD_HASH_TOO_LONG",
            $"Password hash cannot exceed {maxLength} characters"
        );

    public static Error CannotLogin(UserStatus status) =>
        Error.Failure("USER.CANNOT_LOGIN", GetCannotLoginMessage(status));

    public static Error EmailNotConfirmed() =>
        Error.Forbidden("USER.EMAIL_NOT_CONFIRMED", "Email address must be confirmed before login");

    public static Error UserNotActive() =>
        Error.Forbidden("USER.NOT_ACTIVE", "The user is not active");

    public static Error InvalidCredentials() =>
        Error.Unauthorized("USER.INVALID_CREDENTIALS", "The credentials provided are invalid");

    public static Error AlreadyConfirmed() =>
        Error.Failure("USER.ALREADY_CONFIRMED", "Email address is already confirmed");

    public static Error CannotActivateDeletedUser() =>
        Error.Failure("USER.CANNOT_ACTIVATE_DELETED", "Cannot activate a deleted user account");

    public static Error AlreadyInStatus(UserStatus status) =>
        Error.Failure("USER.ALREADY_IN_STATUS", $"User is already in {status} status");

    public static Error InvalidTokenEmailConfirm() =>
        Error.Validation(
            "USER.INVALID_TOKEN_EMAIL_CONFIRM",
            "The email confirmation token is invalid or has expired"
        );

    public static Error InvalidTokenResetPassword() =>
        Error.Validation(
            "USER.INVALID_TOKEN_RESET_PASSWORD",
            "The password reset token is invalid or has expired"
        );

    public static Error InvalidStatusTransition(UserStatus from, UserStatus to) =>
        Error.Failure(
            "USER.INVALID_STATUS_TRANSITION",
            $"Cannot change status from {from} to {to}"
        );

    private static string GetCannotLoginMessage(UserStatus status) =>
        status switch
        {
            UserStatus.Inactive => "User account is inactive",
            UserStatus.Suspended => "User account is suspended",
            UserStatus.PendingEmailConfirmation => "Email confirmation is required before login",
            UserStatus.Deleted => "User account has been deleted",
            _ => "User cannot login due to account status",
        };
}
