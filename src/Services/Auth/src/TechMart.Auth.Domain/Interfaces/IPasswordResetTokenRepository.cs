using TechMart.Auth.Domain.Entities;

namespace TechMart.Auth.Domain.Interfaces;

public interface IPasswordResetTokenRepository
{
    Task<PasswordResetToken> CreateAsync(PasswordResetToken token);
    Task<PasswordResetToken?> GetByTokenAsync(string token);
    Task<PasswordResetToken> UpdateAsync(PasswordResetToken token);
    Task CleanupExpiredTokensAsync();
}
