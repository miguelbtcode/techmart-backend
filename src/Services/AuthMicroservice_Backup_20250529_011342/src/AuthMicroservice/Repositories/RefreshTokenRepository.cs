using AuthMicroservice.Data;
using AuthMicroservice.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthMicroservice.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AuthDbContext _context;

    public RefreshTokenRepository(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken> CreateAsync(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();
        return refreshToken;
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _context
            .RefreshTokens.Include(rt => rt.User)
            .FirstOrDefaultAsync(rt =>
                rt.Token == token && !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow
            );
    }

    public async Task<bool> RevokeTokenAsync(string token)
    {
        var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt =>
            rt.Token == token
        );

        if (refreshToken == null)
            return false;

        refreshToken.IsRevoked = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RevokeAllUserTokensAsync(int userId)
    {
        var tokens = await _context
            .RefreshTokens.Where(rt => rt.UserId == userId && !rt.IsRevoked)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.IsRevoked = true;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task CleanupExpiredTokensAsync()
    {
        var expiredTokens = await _context
            .RefreshTokens.Where(rt => rt.ExpiresAt <= DateTime.UtcNow)
            .ToListAsync();

        _context.RefreshTokens.RemoveRange(expiredTokens);
        await _context.SaveChangesAsync();
    }
}
