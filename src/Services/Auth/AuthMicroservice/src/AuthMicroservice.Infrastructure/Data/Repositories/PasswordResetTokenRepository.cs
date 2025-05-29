using AuthMicroservice.Domain.Entities;
using AuthMicroservice.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuthMicroservice.Infrastructure.Data.Repositories;

public class PasswordResetTokenRepository : IPasswordResetTokenRepository
{
    private readonly AuthDbContext _context;

    public PasswordResetTokenRepository(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<PasswordResetToken> CreateAsync(PasswordResetToken token)
    {
        _context.PasswordResetTokens.Add(token);
        await _context.SaveChangesAsync();
        return token;
    }

    public async Task<PasswordResetToken?> GetByTokenAsync(string token)
    {
        return await _context
            .PasswordResetTokens.Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == token && t.IsValid);
    }

    public async Task<PasswordResetToken> UpdateAsync(PasswordResetToken token)
    {
        _context.PasswordResetTokens.Update(token);
        await _context.SaveChangesAsync();
        return token;
    }

    public async Task CleanupExpiredTokensAsync()
    {
        var expiredTokens = await _context
            .PasswordResetTokens.Where(t => t.ExpiresAt <= DateTime.UtcNow)
            .ToListAsync();

        _context.PasswordResetTokens.RemoveRange(expiredTokens);
        await _context.SaveChangesAsync();
    }
}
