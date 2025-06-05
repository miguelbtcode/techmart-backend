using AuthMicroservice.Domain.Entities;
using AuthMicroservice.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuthMicroservice.Infrastructure.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AuthDbContext _context;

    public UserRepository(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u =>
            u.Username.ToLower() == username.ToLower()
        );
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context
            .Users.Include(u => u.SocialLogins)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User> CreateAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _context.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower());
    }

    public async Task<User?> GetBySocialLoginAsync(string provider, string providerUserId)
    {
        return await _context
            .Users.Include(u => u.SocialLogins)
            .FirstOrDefaultAsync(u =>
                u.SocialLogins.Any(sl =>
                    sl.Provider == provider && sl.ProviderUserId == providerUserId
                )
            );
    }
}
