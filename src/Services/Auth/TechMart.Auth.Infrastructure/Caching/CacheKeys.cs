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

    // Blocked Identifiers (NEW)
    public static string BlockedIdentifier(string identifier) =>
        $"auth:blocked:{identifier.ToLowerInvariant()}";

    // User Cache
    public static string User(Guid userId) => $"auth:user:{userId}";

    public static string UserByEmail(string email) => $"auth:user:email:{email.ToLowerInvariant()}";

    // Roles Cache
    public static string UserRoles(Guid userId) => $"auth:user_roles:{userId}";

    public static string Role(Guid roleId) => $"auth:role:{roleId}";

    // Patterns for bulk operations
    public static string UserPattern(Guid userId) => $"auth:*:{userId}*";

    // Rate Limiting Patterns (NEW)
    public static string LoginAttemptsPattern() => "auth:login_attempts:*";

    public static string BlockedIdentifiersPattern() => "auth:blocked:*";

    // Session Management (NEW - for future use)
    public static string UserSession(Guid userId, string sessionId) =>
        $"auth:session:{userId}:{sessionId}";

    public static string UserSessionsPattern(Guid userId) => $"auth:session:{userId}:*";

    // Device Tracking (NEW - for future use)
    public static string UserDevice(Guid userId, string deviceId) =>
        $"auth:device:{userId}:{deviceId}";

    public static string UserDevicesPattern(Guid userId) => $"auth:device:{userId}:*";

    // Audit Trail (NEW - for future use)
    public static string UserAuditLog(Guid userId) => $"auth:audit:{userId}";

    // Security Events (NEW - for future use)
    public static string SecurityEvent(string eventType, string identifier) =>
        $"auth:security:{eventType}:{identifier.ToLowerInvariant()}";

    // Two-Factor Authentication (NEW - for future use)
    public static string TwoFactorToken(Guid userId) => $"auth:2fa:{userId}";

    public static string TwoFactorBackupCodes(Guid userId) => $"auth:2fa_backup:{userId}";

    // Account Recovery (NEW - for future use)
    public static string RecoveryToken(string email) => $"auth:recovery:{email.ToLowerInvariant()}";

    // IP Reputation (NEW - for future use)
    public static string IpReputation(string ipAddress) => $"auth:ip_reputation:{ipAddress}";

    // Geographic Location Cache (NEW - for future use)
    public static string IpLocation(string ipAddress) => $"auth:ip_location:{ipAddress}";
}
