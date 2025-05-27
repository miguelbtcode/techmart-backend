namespace TechMart.Auth.Infrastructure.Caching;

public static class CacheKeys
{
    // Refresh Tokens
    public static string RefreshToken(Guid userId) => $"auth:refresh_token:{userId}";

    public static string RefreshTokenReverse(string tokenHash) =>
        $"auth:refresh_reverse:{tokenHash}";

    // Password Reset Tokens
    public static string PasswordResetToken(string userIdOrEmail) =>
        $"auth:password_reset:{userIdOrEmail.ToLowerInvariant()}";

    public static string PasswordResetPattern() => "auth:password_reset:*";

    // Email Confirmation Tokens
    public static string EmailConfirmationToken(string email) =>
        $"auth:email_confirm:{email.ToLowerInvariant()}";

    // Rate Limiting
    public static string LoginAttempts(string identifier) => $"auth:login_attempts:{identifier}";

    // User Cache
    public static string User(Guid userId) => $"auth:user:{userId}";

    public static string UserByEmail(string email) => $"auth:user:email:{email.ToLowerInvariant()}";

    // Roles Cache
    public static string UserRoles(Guid userId) => $"auth:user_roles:{userId}";

    public static string Role(Guid roleId) => $"auth:role:{roleId}";

    // Patterns for bulk operations
    public static string UserPattern(Guid userId) => $"auth:*:{userId}*";
}
