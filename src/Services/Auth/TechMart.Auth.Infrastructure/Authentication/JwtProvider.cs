using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TechMart.Auth.Application.Contracts.Authentication;
using TechMart.Auth.Domain.Users.Entities;
using TechMart.Auth.Infrastructure.Settings;
using TokenValidationResult = TechMart.Auth.Application.Contracts.Authentication.TokenValidationResult;

namespace TechMart.Auth.Infrastructure.Authentication;

public sealed class JwtProvider : IJwtProvider
{
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<JwtProvider> _logger;
    private readonly TokenValidationParameters _validationParameters;

    public JwtProvider(IOptions<JwtSettings> jwtSettings, ILogger<JwtProvider> logger)
    {
        _jwtSettings = jwtSettings.Value;
        _logger = logger;

        // Pre-configure validation parameters for reuse
        var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);
        _validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = _jwtSettings.ValidateIssuer,
            ValidIssuer = _jwtSettings.Issuer,
            ValidateAudience = _jwtSettings.ValidateAudience,
            ValidAudience = _jwtSettings.Audience,
            ValidateLifetime = _jwtSettings.ValidateLifetime,
            ClockSkew = TimeSpan.FromSeconds(_jwtSettings.ClockSkewSeconds),
            RequireExpirationTime = true,
            RequireSignedTokens = true,
        };
    }

    public string GenerateToken(User user)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.Value.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email.Value),
                new(JwtRegisteredClaimNames.GivenName, user.FirstName),
                new(JwtRegisteredClaimNames.FamilyName, user.LastName),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(
                    JwtRegisteredClaimNames.Iat,
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64
                ),
                // Custom claims
                new(ClaimTypes.NameIdentifier, user.Id.Value.ToString()),
                new(ClaimTypes.Email, user.Email.Value),
                new(ClaimTypes.Name, user.GetFullName()),
                new("user_status", user.Status.ToString()),
                new("email_confirmed", user.IsEmailConfirmed.ToString().ToLower()),
            };

            // Add roles
            foreach (var role in user.GetRoleNames())
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                NotBefore = DateTime.UtcNow,
                IssuedAt = DateTime.UtcNow,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                ),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            _logger.LogDebug("JWT token generated successfully for user {UserId}", user.Id.Value);
            return tokenString;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate JWT token for user {UserId}", user.Id.Value);
            throw new InvalidOperationException("Token generation failed", ex);
        }
    }

    public async Task<TokenValidationResult> ValidateAccessTokenAsync(
        string accessToken,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return new TokenValidationResult(false, null, null, null, "Token is null or empty");
        }

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var principal = tokenHandler.ValidateToken(
                accessToken,
                _validationParameters,
                out var validatedToken
            );

            var jwtToken = validatedToken as JwtSecurityToken;
            if (jwtToken == null)
            {
                return new TokenValidationResult(false, null, null, null, "Invalid token format");
            }

            // Extract claims
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var roles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray();

            var isValidUserId = Guid.TryParse(userIdClaim, out var userId);
            if (!isValidUserId)
            {
                return new TokenValidationResult(
                    false,
                    null,
                    null,
                    null,
                    "Invalid user ID in token"
                );
            }

            _logger.LogDebug("JWT token validated successfully for user {UserId}", userId);

            return new TokenValidationResult(
                IsValid: true,
                UserId: userId,
                ExpiresAt: jwtToken.ValidTo,
                Roles: roles
            );
        }
        catch (SecurityTokenExpiredException)
        {
            _logger.LogWarning("JWT token has expired");
            return new TokenValidationResult(false, null, null, null, "Token has expired");
        }
        catch (SecurityTokenException ex)
        {
            _logger.LogWarning(ex, "JWT token validation failed due to security token exception");
            return new TokenValidationResult(false, null, null, null, "Invalid token");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during JWT token validation");
            return new TokenValidationResult(false, null, null, null, "Token validation failed");
        }
    }

    public async Task<IEnumerable<Claim>> GetClaimsFromTokenAsync(
        string accessToken,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return Enumerable.Empty<Claim>();
        }

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            // Validate without checking expiration for claim extraction
            var validationParams = _validationParameters.Clone();
            validationParams.ValidateLifetime = false;

            var principal = tokenHandler.ValidateToken(
                accessToken,
                validationParams,
                out var validatedToken
            );

            return principal.Claims;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to extract claims from token");
            return Enumerable.Empty<Claim>();
        }
    }

    public async Task<Guid?> GetUserIdFromTokenAsync(
        string accessToken,
        CancellationToken cancellationToken = default
    )
    {
        var claims = await GetClaimsFromTokenAsync(accessToken, cancellationToken);
        var userIdClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(userIdClaim))
        {
            return null;
        }

        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}

// Helper extension for TokenValidationParameters
public static class TokenValidationParametersExtensions
{
    public static TokenValidationParameters Clone(this TokenValidationParameters source)
    {
        return new TokenValidationParameters
        {
            ValidateIssuerSigningKey = source.ValidateIssuerSigningKey,
            IssuerSigningKey = source.IssuerSigningKey,
            ValidateIssuer = source.ValidateIssuer,
            ValidIssuer = source.ValidIssuer,
            ValidateAudience = source.ValidateAudience,
            ValidAudience = source.ValidAudience,
            ValidateLifetime = source.ValidateLifetime,
            ClockSkew = source.ClockSkew,
            RequireExpirationTime = source.RequireExpirationTime,
            RequireSignedTokens = source.RequireSignedTokens,
        };
    }
}
