using TechMart.Auth.Domain.Entities;
using TechMart.Auth.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace TechMart.Auth.Infrastructure.Data.Repositories;

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
            .FirstOrDefaultAsync(rt => rt.Token == token && rt.IsActive);
    }

    public async Task<bool> RevokeTokenAsync(string token)
    {
        var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt =>
            rt.Token == token
        );

        if (refreshToken == null)
            return false;

        refreshToken.Revoke();
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RevokeAllUserTokensAsync(int userId)
    {
        var tokens = await _context
            .RefreshTokens.Where(rt => rt.UserId == userId && rt.IsActive)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.Revoke();
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
