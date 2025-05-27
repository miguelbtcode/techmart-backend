using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TechMart.Auth.Application.Abstractions.Authentication;
using TechMart.Auth.Domain.Users.Entities;
using TechMart.Auth.Infrastructure.Settings;
using TokenValidationResult = TechMart.Auth.Application.Abstractions.Authentication.TokenValidationResult;

namespace TechMart.Auth.Infrastructure.Authentication;

public class JwtProvider : IJwtProvider
{
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<JwtProvider> _logger;

    public JwtProvider(IOptions<JwtSettings> jwtSettings, ILogger<JwtProvider> logger)
    {
        _jwtSettings = jwtSettings.Value;
        _logger = logger;
    }

    /// <summary>
    /// Genera JWT Access Token (stateless)
    /// </summary>
    public string GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.Value.ToString()),
            new(ClaimTypes.Email, user.Email.Value),
            new(ClaimTypes.Name, user.GetFullName()),
            new("user_status", user.Status.ToString()),
        };

        // Agregar roles como claims
        foreach (var role in user.GetRoleNames())
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            ),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    /// <summary>
    /// Valida JWT Access Token
    /// </summary>
    public async Task<TokenValidationResult> ValidateAccessTokenAsync(
        string accessToken,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
            };

            var principal = tokenHandler.ValidateToken(
                accessToken,
                validationParameters,
                out var validatedToken
            );

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            var roles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value);

            return new TokenValidationResult(
                IsValid: true,
                UserId: userIdClaim != null ? Guid.Parse(userIdClaim.Value) : null,
                ExpiresAt: validatedToken.ValidTo,
                Roles: roles
            );
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "JWT validation failed");
            return new TokenValidationResult(false, null, null);
        }
    }
}
