using TechMart.Auth.Domain.Entities;

namespace TechMart.Auth.Domain.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken> CreateAsync(RefreshToken refreshToken);
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task<bool> RevokeTokenAsync(string token);
    Task<bool> RevokeAllUserTokensAsync(int userId);
    Task CleanupExpiredTokensAsync();
}
