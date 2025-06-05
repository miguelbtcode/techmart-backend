using TechMart.Auth.Domain.Entities;

namespace TechMart.Auth.Domain.Interfaces;

public interface IEmailVerificationTokenRepository
{
    Task<EmailVerificationToken> CreateAsync(EmailVerificationToken token);
    Task<EmailVerificationToken?> GetByTokenAsync(string token);
    Task<EmailVerificationToken> UpdateAsync(EmailVerificationToken token);
    Task CleanupExpiredTokensAsync();
}
