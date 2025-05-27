namespace TechMart.Auth.Infrastructure.Settings;

/// <summary>
/// Configuración para JWT (JSON Web Tokens)
/// </summary>
public sealed class JwtSettings
{
    public const string SectionName = "JwtSettings";

    /// <summary>
    /// Clave secreta para firmar los tokens JWT
    /// Debe ser al menos de 32 caracteres para HS256
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// Emisor del token (quien lo genera)
    /// Ejemplo: "TechMart.Auth"
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Audiencia del token (para quién es válido)
    /// Ejemplo: "TechMart.API"
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Duración en minutos del Access Token
    /// Recomendado: 15-60 minutos
    /// </summary>
    public int AccessTokenExpirationMinutes { get; set; } = 60;

    /// <summary>
    /// Duración en días del Refresh Token
    /// Recomendado: 7-30 días
    /// </summary>
    public int RefreshTokenExpirationDays { get; set; } = 7;

    /// <summary>
    /// Algoritmo de firma a usar
    /// Por defecto: HS256
    /// </summary>
    public string Algorithm { get; set; } = "HS256";

    /// <summary>
    /// Si validar la audiencia en los tokens
    /// </summary>
    public bool ValidateAudience { get; set; } = true;

    /// <summary>
    /// Si validar el emisor en los tokens
    /// </summary>
    public bool ValidateIssuer { get; set; } = true;

    /// <summary>
    /// Si validar el tiempo de vida del token
    /// </summary>
    public bool ValidateLifetime { get; set; } = true;

    /// <summary>
    /// Margen de tiempo para validación (skew)
    /// Útil para sincronización entre servidores
    /// </summary>
    public int ClockSkewSeconds { get; set; } = 0;

    /// <summary>
    /// Valida que la configuración sea correcta
    /// </summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(SecretKey))
            throw new ArgumentException("JWT SecretKey is required");

        if (SecretKey.Length < 32)
            throw new ArgumentException("JWT SecretKey must be at least 32 characters long");

        if (string.IsNullOrWhiteSpace(Issuer))
            throw new ArgumentException("JWT Issuer is required");

        if (string.IsNullOrWhiteSpace(Audience))
            throw new ArgumentException("JWT Audience is required");

        if (AccessTokenExpirationMinutes <= 0)
            throw new ArgumentException("AccessTokenExpirationMinutes must be greater than 0");

        if (RefreshTokenExpirationDays <= 0)
            throw new ArgumentException("RefreshTokenExpirationDays must be greater than 0");
    }
}
