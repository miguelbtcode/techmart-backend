using TechMart.Auth.Domain.Entities;
using TechMart.Auth.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace TechMart.Auth.Infrastructure.Data.Repositories;

public class EmailVerificationTokenRepository : IEmailVerificationTokenRepository
{
    private readonly AuthDbContext _context;

    public EmailVerificationTokenRepository(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<EmailVerificationToken> CreateAsync(EmailVerificationToken token)
    {
        _context.EmailVerificationTokens.Add(token);
        await _context.SaveChangesAsync();
        return token;
    }

    public async Task<EmailVerificationToken?> GetByTokenAsync(string token)
    {
        return await _context
            .EmailVerificationTokens.Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == token && t.IsValid);
    }

    public async Task<EmailVerificationToken> UpdateAsync(EmailVerificationToken token)
    {
        _context.EmailVerificationTokens.Update(token);
        await _context.SaveChangesAsync();
        return token;
    }

    public async Task CleanupExpiredTokensAsync()
    {
        var expiredTokens = await _context
            .EmailVerificationTokens.Where(t => t.ExpiresAt <= DateTime.UtcNow)
            .ToListAsync();

        _context.EmailVerificationTokens.RemoveRange(expiredTokens);
        await _context.SaveChangesAsync();
    }
}
