using System.Security.Claims;
using AuthMicroservice.Domain.Entities;

namespace AuthMicroservice.Application.Contracts.Jwt;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal? ValidateToken(string token);
    DateTime GetAccessTokenExpiration(); // Nuevo método
    int GetRefreshTokenExpirationDays(bool rememberMe = false); // Nuevo método
}
