using AuthMicroservice.Domain.Entities;

namespace AuthMicroservice.Domain.Interfaces;

public interface IPasswordResetTokenRepository
{
    Task<PasswordResetToken> CreateAsync(PasswordResetToken token);
    Task<PasswordResetToken?> GetByTokenAsync(string token);
    Task<PasswordResetToken> UpdateAsync(PasswordResetToken token);
    Task CleanupExpiredTokensAsync();
}
