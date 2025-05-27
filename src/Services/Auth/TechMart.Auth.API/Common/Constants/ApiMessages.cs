namespace TechMart.Auth.API.Common.Constants;

/// <summary>
/// Mensajes estándar de la API
/// </summary>
public static class ApiMessages
{
    // Authentication
    public const string UserRegistered = "User registered successfully";
    public const string UserLoggedIn = "User logged in successfully";
    public const string EmailConfirmed = "Email confirmed successfully";
    public const string PasswordChanged = "Password changed successfully";
    public const string PasswordResetSent = "Password reset email sent successfully";
    public const string PasswordReset = "Password reset successfully";

    // Users
    public const string UserProfileUpdated = "User profile updated successfully";
    public const string UserNotFound = "User not found";
    public const string UsersRetrieved = "Users retrieved successfully";
    public const string UserDetailsRetrieved = "User details retrieved successfully";

    // General
    public const string OperationSuccessful = "Operation completed successfully";
    public const string InvalidCredentials = "Invalid credentials provided";
    public const string AccessDenied = "Access denied";
    public const string ValidationFailed = "Validation failed";
    public const string InternalServerError = "An internal server error occurred";
}
