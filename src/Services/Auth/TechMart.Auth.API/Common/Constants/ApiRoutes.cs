namespace TechMart.Auth.API.Common.Constants;

/// <summary>
/// Rutas estándar de la API
/// </summary>
public static class ApiRoutes
{
    public const string ApiVersion = "v1";
    public const string ApiPrefix = $"api/{ApiVersion}";

    public static class Auth
    {
        public const string Base = "auth";
        public const string Register = "register";
        public const string Login = "login";
        public const string RefreshToken = "refresh";
        public const string ConfirmEmail = "confirm-email";
        public const string ForgotPassword = "forgot-password";
        public const string ResetPassword = "reset-password";
        public const string ChangePassword = "change-password";
        public const string Me = "me";
    }

    public static class Users
    {
        public const string Base = "users";
        public const string Profile = "profile";
        public const string Roles = "roles";
    }

    public static class Health
    {
        public const string Base = "health";
        public const string Ready = "ready";
        public const string Live = "live";
    }
}
